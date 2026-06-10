namespace face_recognition_api.Models;

// Junction table for many-to-many between Employee and Role
public class EmployeeRole
{
    public long EmpId { get; set; }
    public long RoleId { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
