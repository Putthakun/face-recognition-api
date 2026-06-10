namespace face_recognition_api.DTOs;

// Payload sent by the client on login
public class LoginRequestDto
{
    public long EmpId { get; set; }
    public string Password { get; set; } = string.Empty;
}
