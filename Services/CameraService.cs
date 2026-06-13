using face_recognition_api.Data;
using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Services;

public class CameraService : ICameraService
{
    private readonly AppDbContext _db;

    public CameraService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CameraResponseDto> CreateAsync(CreateCameraDto request)
    {
        var camera = new Camera
        {
            Location = request.Location
        };

        _db.Cameras.Add(camera);
        await _db.SaveChangesAsync();

        return MapToDto(camera);
    }

    public async Task<List<CameraResponseDto>> GetAllAsync()
    {
        var cameras = await _db.Cameras.ToListAsync();
        return cameras.Select(MapToDto).ToList();
    }

    public async Task<CameraResponseDto?> UpdateAsync(long cameraId, UpdateCameraDto request)
    {
        var camera = await _db.Cameras.FindAsync(cameraId);
        if (camera == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Location))
            camera.Location = request.Location;

        await _db.SaveChangesAsync();
        return MapToDto(camera);
    }

    public async Task<bool> DeleteAsync(long cameraId)
    {
        var camera = await _db.Cameras.FindAsync(cameraId);
        if (camera == null) return false;

        _db.Cameras.Remove(camera);
        await _db.SaveChangesAsync();
        return true;
    }

    private static CameraResponseDto MapToDto(Camera camera) => new()
    {
        CameraId = camera.CameraId,
        Location = camera.Location
    };
}
