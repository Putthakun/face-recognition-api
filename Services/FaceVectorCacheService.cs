using face_recognition_api.Data;
using face_recognition_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace face_recognition_api.Services;

public class FaceVectorCacheService : IFaceVectorCacheService
{
    // Redis Hash key — stores all face vectors as { empId -> bytes }
    private const string HashKey = "face:vectors";

    private readonly IDatabase _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FaceVectorCacheService> _logger;

    public FaceVectorCacheService(
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        ILogger<FaceVectorCacheService> logger)
    {
        _redis       = redis.GetDatabase();
        _scopeFactory = scopeFactory;
        _logger      = logger;
    }

    /// <summary>Called once on startup — loads all vectors from DB into Redis</summary>
    public async Task LoadFromDbAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var embeddings = await db.FaceEmbeddeds
            .Where(f => f.FaceEmbeddedData != null)
            .ToListAsync();

        if (embeddings.Count == 0)
        {
            _logger.LogInformation("No face vectors found in DB — cache is empty");
            return;
        }

        // Write all vectors into Redis Hash in one batch
        var entries = embeddings
            .Select(f => new HashEntry(f.EmpId.ToString(), f.FaceEmbeddedData))
            .ToArray();

        await _redis.HashSetAsync(HashKey, entries);
        _logger.LogInformation("Loaded {Count} face vectors into Redis", embeddings.Count);
    }

    /// <summary>Add or update a single vector in cache</summary>
    public async Task SetAsync(long empId, float[] vector)
    {
        var bytes = FloatToBytes(vector);
        await _redis.HashSetAsync(HashKey, empId.ToString(), bytes);
    }

    /// <summary>Returns all cached vectors — key = EmpId</summary>
    public async Task<Dictionary<long, float[]>> GetAllAsync()
    {
        var entries = await _redis.HashGetAllAsync(HashKey);

        return entries.ToDictionary(
            e => long.Parse(e.Name!),
            e => BytesToFloat((byte[])e.Value!)
        );
    }

    /// <summary>Remove a vector from cache when employee is deleted</summary>
    public async Task RemoveAsync(long empId)
    {
        await _redis.HashDeleteAsync(HashKey, empId.ToString());
    }

    // float[] → byte[] (4 bytes per float)
    private static byte[] FloatToBytes(float[] vector)
    {
        var bytes = new byte[vector.Length * 4];
        Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    // byte[] → float[]
    private static float[] BytesToFloat(byte[] bytes)
    {
        var vector = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, vector, 0, bytes.Length);
        return vector;
    }
}
