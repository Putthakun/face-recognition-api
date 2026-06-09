namespace face_recognition_api.Configurations;

// class นี้ map กับ "JwtSettings" ใน appsettings.json
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; } = 24;
}
