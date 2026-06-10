using Microsoft.AspNetCore.Http;

namespace face_recognition_api.DTOs;

// DTO used by Admin to create a new employee account
public class CreateUserDto
{
    public long EmpId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Role { get; set; } = string.Empty;
    public IFormFile? Photo { get; set; } // optional face photo
}
