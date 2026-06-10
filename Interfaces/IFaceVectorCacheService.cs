namespace face_recognition_api.Interfaces;

public interface IFaceVectorCacheService
{
    /// <summary>Load all face vectors from DB into Redis (called on startup)</summary>
    Task LoadFromDbAsync();

    /// <summary>Cache a single vector when new employee is added</summary>
    Task SetAsync(long empId, float[] vector);

    /// <summary>Get all cached vectors for face matching — key = EmpId</summary>
    Task<Dictionary<long, float[]>> GetAllAsync();

    /// <summary>Remove vector when employee is deleted</summary>
    Task RemoveAsync(long empId);
}
