namespace face_recognition_api.DTOs;

// สิ่งที่ Server ส่งกลับหลัง Login สำเร็จ
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
