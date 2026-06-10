namespace face_recognition_api.Models;

public class Camera
{
    public long CameraId { get; set; }
    public string Location { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
