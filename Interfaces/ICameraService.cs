using face_recognition_api.DTOs;

namespace face_recognition_api.Interfaces;

public interface ICameraService
{
    Task<CameraResponseDto> CreateAsync(CreateCameraDto request);
    Task<List<CameraResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(long cameraId);
}
