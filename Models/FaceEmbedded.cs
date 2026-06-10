namespace face_recognition_api.Models;

public class FaceEmbedded
{
    public long FaceEmbeddedId { get; set; }
    public long EmpId { get; set; }

    // 512-dimension vector stored as byte array (512 floats × 4 bytes = 2048 bytes)
    public byte[]? FaceEmbeddedData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee Employee { get; set; } = null!;
}
