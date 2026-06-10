namespace face_recognition_api.Models;

public class Role
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    // Navigation property
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
}
