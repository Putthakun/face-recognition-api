namespace face_recognition_api.DTOs;

// Payload returned by the server after a successful login
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
