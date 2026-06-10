namespace face_recognition_api.Interfaces;

public interface IFaceRecognitionService
{
    /// <summary>
    /// Sends photo to Face API and returns embedding vector.
    /// Returns null if no face detected or request failed.
    /// </summary>
    Task<float[]?> GetEmbeddingAsync(IFormFile photo);
}
