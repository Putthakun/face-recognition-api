using face_recognition_api.DTOs;

namespace face_recognition_api.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
