namespace face_recognition_api.DTOs;

public class EmployeeResponseDto
{
    public long EmpId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}
