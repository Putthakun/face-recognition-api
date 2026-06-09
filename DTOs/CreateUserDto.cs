namespace face_recognition_api.DTOs;

// DTO used by Admin to create a new employee account
public class CreateUserDto
{
    public string EmpId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
}
