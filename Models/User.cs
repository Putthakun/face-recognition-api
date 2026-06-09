namespace face_recognition_api.Models;

public class User
{
    public int Id { get; set; }
    public string EmpId { get; set; } = string.Empty;

    // Store hashed password only, never plain text
    public string PasswordHash { get; set; } = string.Empty;

    // "Admin" or "Employee"
    public string Role { get; set; } = "Employee";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
