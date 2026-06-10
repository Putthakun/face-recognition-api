namespace face_recognition_api.Models;

public class Credential
{
    public long CredentialId { get; set; }
    public long EmpId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // "Admin" or "Employee" — system access level
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee Employee { get; set; } = null!;
}
