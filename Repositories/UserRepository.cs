using face_recognition_api.Interfaces;
using face_recognition_api.Models;

namespace face_recognition_api.Repositories;

// ทำตามสัญญาใน IUserRepository
// ตอนนี้ใช้ in-memory list แทน DB ก่อน (เพิ่ม DB จริงทีหลัง)
public class UserRepository : IUserRepository
{
    // จำลอง DB ด้วย List ไว้ก่อน
    private static readonly List<User> _users = new();

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        user.Id = _users.Count + 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<List<User>> GetAllAsync()
    {
        return Task.FromResult(_users.ToList());
    }

    public Task<bool> AnyAsync()
    {
        return Task.FromResult(_users.Any());
    }
}
