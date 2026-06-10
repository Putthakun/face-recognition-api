namespace face_recognition_api.Models;

public class Transaction
{
    public long TransactionId { get; set; }
    public long? EmpId { get; set; }       // nullable — face detected but unknown person
    public long? CameraId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Employee? Employee { get; set; }
    public Camera? Camera { get; set; }
}
