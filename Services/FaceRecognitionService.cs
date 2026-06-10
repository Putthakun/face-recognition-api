using face_recognition_api.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace face_recognition_api.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    private readonly HttpClient _http;
    private readonly ILogger<FaceRecognitionService> _logger;

    public FaceRecognitionService(HttpClient http, ILogger<FaceRecognitionService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<float[]?> GetEmbeddingAsync(IFormFile photo)
    {
        using var content = new MultipartFormDataContent();
        using var stream = photo.OpenReadStream();
        content.Add(new StreamContent(stream), "file", photo.FileName);

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsync("/api/embeddings", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reach Face API");
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Face API returned {StatusCode}", response.StatusCode);
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<EmbedResponse>();

        if (body is null || !body.FaceDetected)
        {
            _logger.LogWarning("Face API: no face detected in photo");
            return null;
        }

        return body.Embedding;
    }
}

// Maps to: { "face_detected": true, "embedding": [...] }
file record EmbedResponse(
    [property: JsonPropertyName("face_detected")] bool FaceDetected,
    [property: JsonPropertyName("embedding")]     float[]? Embedding
);
