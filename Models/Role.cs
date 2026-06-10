namespace face_recognition_api.Models;

public class Role
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsSystem { get; set; } = false; // true = requires password (Admin, Supervisor)

    // Navigation property
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
}
