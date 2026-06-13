namespace face_recognition_api.DTOs;

public class CreateCameraDto
{
    public string Location { get; set; } = string.Empty;
}

// All fields optional — only provided fields are updated
public class UpdateCameraDto
{
    public string? Location { get; set; }
}

public class CameraResponseDto
{
    public long CameraId { get; set; }
    public string Location { get; set; } = string.Empty;
}
