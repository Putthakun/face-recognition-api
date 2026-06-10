using face_recognition_api.Data;
using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly IFaceRecognitionService _faceService;
    private readonly IFaceVectorCacheService _cache;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        AppDbContext db,
        IFaceRecognitionService faceService,
        IFaceVectorCacheService cache,
        ILogger<EmployeeService> logger)
    {
        _db          = db;
        _faceService = faceService;
        _cache       = cache;
        _logger      = logger;
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateUserDto request)
    {
        // 1. Check duplicate EmpId
        if (await _db.Employees.AnyAsync(e => e.EmpId == request.EmpId))
            throw new InvalidOperationException($"Employee {request.EmpId} already exists");

        // 2. Create Employee
        var employee = new Employee
        {
            EmpId = request.EmpId,
            Name  = request.Name
        };

        // 2. Create Credential if password provided
        Credential? credential = null;
        if (!string.IsNullOrEmpty(request.Password))
        {
            credential = new Credential
            {
                EmpId         = request.EmpId,
                PasswordHash  = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role          = request.Role
            };
        }

        _db.Employees.Add(employee);
        if (credential != null) _db.Credentials.Add(credential);
        await _db.SaveChangesAsync();

        // 3. Send photo to Face API → save vector to DB + Redis
        if (request.Photo != null)
        {
            var vector = await _faceService.GetEmbeddingAsync(request.Photo);
            if (vector != null)
            {
                _db.FaceEmbeddeds.Add(new FaceEmbedded
                {
                    EmpId            = request.EmpId,
                    FaceEmbeddedData = ConvertToBytes(vector)
                });
                await _db.SaveChangesAsync();
                await _cache.SetAsync(request.EmpId, vector); // update Redis cache
            }
            else
            {
                _logger.LogWarning("No face detected for EmpId {EmpId} — embedding not saved", request.EmpId);
            }
        }

        return MapToDto(employee, credential);
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _db.Employees
            .Include(e => e.Credential)
            .ToListAsync();

        return employees.Select(e => MapToDto(e, e.Credential)).ToList();
    }

    public async Task<bool> DeleteAsync(long empId)
    {
        var employee = await _db.Employees.FindAsync(empId);
        if (employee == null) return false;

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        await _cache.RemoveAsync(empId); // remove from Redis cache
        return true;
    }

    // Converts float[] to byte[] for varbinary storage (4 bytes per float)
    private static byte[] ConvertToBytes(float[] vector)
    {
        var bytes = new byte[vector.Length * 4];
        Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static EmployeeResponseDto MapToDto(Employee employee, Credential? credential) => new()
    {
        EmpId     = employee.EmpId,
        Name      = employee.Name,
        Role      = credential?.Role,
        IsActive  = credential?.IsActive,
        CreatedAt = credential?.CreatedAt
    };
}
