namespace face_recognition_api.Models;

public class Employee
{
    public long EmpId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties (EF Core uses these to understand relationships)
    public Credential? Credential { get; set; }
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
    public ICollection<FaceEmbedded> FaceEmbeddeds { get; set; } = new List<FaceEmbedded>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
