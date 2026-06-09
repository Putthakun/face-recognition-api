namespace face_recognition_api.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    // Hash password
    public string PasswordHash { get; set; } = string.Empty;

    // "Admin" หรือ "Employee"
    public string Role { get; set; } = "Employee";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
