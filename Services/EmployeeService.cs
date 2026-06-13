using face_recognition_api.Data;
using face_recognition_api.DTOs;
using face_recognition_api.Exceptions;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly IFaceRecognitionService _faceService;
    private readonly IFaceVectorCacheService _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        AppDbContext db,
        IFaceRecognitionService faceService,
        IFaceVectorCacheService cache,
        IHttpClientFactory httpClientFactory,
        ILogger<EmployeeService> logger)
    {
        _db                 = db;
        _faceService        = faceService;
        _cache              = cache;
        _httpClientFactory  = httpClientFactory;
        _logger             = logger;
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateUserDto request)
    {
        // 1. Check duplicate EmpId
        if (await _db.Employees.AnyAsync(e => e.EmpId == request.EmpId))
            throw new InvalidOperationException($"Employee {request.EmpId} already exists");

        // 2. Extract face vector FIRST (before writing anything to DB)
        //    so a bad photo doesn't leave behind a half-created employee.
        float[]? vector = null;
        if (request.Photo != null)
        {
            vector = await _faceService.GetEmbeddingAsync(request.Photo);
            if (vector == null)
            {
                _logger.LogWarning("No face detected in uploaded photo for new EmpId {EmpId}", request.EmpId);
                throw new FaceNotDetectedException("ไม่พบใบหน้าในรูปภาพ กรุณาอัปโหลดรูปภาพที่เห็นใบหน้าชัดเจน");
            }
        }

        // 3. Create Employee
        var employee = new Employee
        {
            EmpId = request.EmpId,
            Name  = request.Name
        };

        // 4. Create Credential if password provided
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

        // 5. Save vector to DB + Redis
        if (vector != null)
        {
            _db.FaceEmbeddeds.Add(new FaceEmbedded
            {
                EmpId            = request.EmpId,
                FaceEmbeddedData = ConvertToBytes(vector)
            });
            await _db.SaveChangesAsync();
            await _cache.SetAsync(request.EmpId, vector); // update Redis cache
            await NotifyFaceServerReloadAsync();           // tell Python server to reload
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

    public async Task<EmployeeResponseDto?> UpdateAsync(long empId, UpdateUserDto request)
    {
        var employee = await _db.Employees
            .Include(e => e.Credential)
            .FirstOrDefaultAsync(e => e.EmpId == empId);

        if (employee == null) return null;

        // 0. Extract face vector FIRST (before applying any other changes)
        //    so a bad photo doesn't leave the record partially updated.
        float[]? vector = null;
        if (request.Photo != null)
        {
            vector = await _faceService.GetEmbeddingAsync(request.Photo);
            if (vector == null)
            {
                _logger.LogWarning("No face detected in updated photo for EmpId {EmpId}", empId);
                throw new FaceNotDetectedException("ไม่พบใบหน้าในรูปภาพ กรุณาอัปโหลดรูปภาพที่เห็นใบหน้าชัดเจน");
            }
        }

        // 1. Update Name
        if (!string.IsNullOrWhiteSpace(request.Name))
            employee.Name = request.Name;

        // 2. Update / create Credential (Password, Role, IsActive)
        var credential = employee.Credential;

        if (!string.IsNullOrEmpty(request.Password) || !string.IsNullOrEmpty(request.Role) || request.IsActive.HasValue)
        {
            if (credential == null)
            {
                credential = new Credential
                {
                    EmpId        = empId,
                    PasswordHash = !string.IsNullOrEmpty(request.Password)
                        ? BCrypt.Net.BCrypt.HashPassword(request.Password)
                        : BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")), // placeholder, must be changed
                    Role = request.Role ?? "Employee",
                };
                _db.Credentials.Add(credential);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Password))
                    credential.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                if (!string.IsNullOrEmpty(request.Role))
                    credential.Role = request.Role;

                if (request.IsActive.HasValue)
                    credential.IsActive = request.IsActive.Value;
            }
        }

        await _db.SaveChangesAsync();

        // 3. Update face vector if a new photo was provided
        if (vector != null)
        {
            var existing = await _db.FaceEmbeddeds.FirstOrDefaultAsync(f => f.EmpId == empId);
            if (existing != null)
            {
                existing.FaceEmbeddedData = ConvertToBytes(vector);
                existing.CreatedAt        = DateTime.UtcNow;
            }
            else
            {
                _db.FaceEmbeddeds.Add(new FaceEmbedded
                {
                    EmpId            = empId,
                    FaceEmbeddedData = ConvertToBytes(vector)
                });
            }
            await _db.SaveChangesAsync();
            await _cache.SetAsync(empId, vector);
            await NotifyFaceServerReloadAsync();
        }

        return MapToDto(employee, credential);
    }

    public async Task<bool> DeleteAsync(long empId)
    {
        var employee = await _db.Employees.FindAsync(empId);
        if (employee == null) return false;

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        await _cache.RemoveAsync(empId); // remove from Redis cache
        await NotifyFaceServerReloadAsync();
        return true;
    }

    private async Task NotifyFaceServerReloadAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FaceApi");
            await client.PostAsync("/api/reload", null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Could not notify face server to reload: {Message}", ex.Message);
        }
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
