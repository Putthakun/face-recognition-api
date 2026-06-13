namespace face_recognition_api.Exceptions;

// Thrown when a face cannot be detected/extracted from an uploaded photo
public class FaceNotDetectedException : Exception
{
    public FaceNotDetectedException(string message) : base(message) { }
}
