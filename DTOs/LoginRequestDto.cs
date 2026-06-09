namespace face_recognition_api.DTOs;

// สิ่งที่ Client ส่งมาตอน Login
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
