using face_recognition_api.DTOs;

namespace face_recognition_api.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeResponseDto> CreateAsync(CreateUserDto request);
    Task<List<EmployeeResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(long empId);
}
