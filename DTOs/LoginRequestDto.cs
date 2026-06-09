namespace face_recognition_api.DTOs;

// Payload sent by the client on login
public class LoginRequestDto
{
    public string EmpId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
