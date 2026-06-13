namespace face_recognition_api.DTOs;

public class TransactionResponseDto
{
    public long TransactionId { get; set; }
    public long? EmpId { get; set; }
    public string? EmpName { get; set; }      // null = unknown person
    public long? CameraId { get; set; }
    public string? CameraLocation { get; set; }
    public DateTime CreatedAt { get; set; }
}
