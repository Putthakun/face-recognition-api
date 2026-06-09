using face_recognition_api.Interfaces;
using face_recognition_api.Models;

namespace face_recognition_api.Repositories;

// Implements IUserRepository using an in-memory list (replace with real DB later)
public class UserRepository : IUserRepository
{
    private static readonly List<User> _users = new();

    public Task<User?> GetByEmpIdAsync(string empId)
    {
        var user = _users.FirstOrDefault(u => u.EmpId == empId);
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
