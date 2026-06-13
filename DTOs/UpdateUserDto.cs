using Microsoft.AspNetCore.Http;

namespace face_recognition_api.DTOs;

// DTO used by Admin to update an existing employee
// All fields optional — only provided fields are updated
public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Password { get; set; }       // if set, replaces password hash
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public IFormFile? Photo { get; set; }        // if set, re-extracts face vector
}
