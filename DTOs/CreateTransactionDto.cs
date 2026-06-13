namespace face_recognition_api.DTOs;

public class CreateTransactionDto
{
    public long? EmpId { get; set; }      // null = face detected but unknown
    public long? CameraId { get; set; }
}
