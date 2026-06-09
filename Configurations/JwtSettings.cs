namespace face_recognition_api.Configurations;

// Maps to the "JwtSettings" section in appsettings.json
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; } = 24;
}
