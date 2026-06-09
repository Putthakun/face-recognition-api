using face_recognition_api.Models;

namespace face_recognition_api.Interfaces;

// สัญญาว่า UserRepository จะต้องทำสิ่งเหล่านี้ได้
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<List<User>> GetAllAsync();
    Task<bool> AnyAsync(); // เช็คว่ามี user ในระบบแล้วไหม (ใช้ตอน seed)
}
