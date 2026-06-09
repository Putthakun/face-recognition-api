using face_recognition_api.Models;

namespace face_recognition_api.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmpIdAsync(string empId);
    Task<User> CreateAsync(User user);
    Task<List<User>> GetAllAsync();
    Task<bool> AnyAsync(); // Returns true if any user exists (used for seeding)
}
