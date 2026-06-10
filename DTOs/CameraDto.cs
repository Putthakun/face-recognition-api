namespace face_recognition_api.DTOs;

public class CreateCameraDto
{
    public string Location { get; set; } = string.Empty;
}

public class CameraResponseDto
{
    public long CameraId { get; set; }
    public string Location { get; set; } = string.Empty;
}
