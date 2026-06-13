using face_recognition_api.Data;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Credential?> GetByEmpIdAsync(long empId)
    {
        return await _db.Credentials
            .Include(c => c.Employee) // load Employee data alongside Credential
            .FirstOrDefaultAsync(c => c.EmpId == empId && c.IsActive);
    }

    public async Task CreateAsync(Employee employee, Credential credential)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(); // save Employee first to get EmpId

        credential.EmpId = employee.EmpId;
        _db.Credentials.Add(credential);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync()
    {
        return await _db.Credentials.AnyAsync();
    }
}
