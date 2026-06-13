using face_recognition_api.Models;

namespace face_recognition_api.Interfaces;

public interface IUserRepository
{
    Task<Credential?> GetByEmpIdAsync(long empId);
    Task CreateAsync(Employee employee, Credential credential);
    Task<bool> AnyAsync();
}
