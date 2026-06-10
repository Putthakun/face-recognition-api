using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/admin/employees")]
[Authorize(Roles = "Admin")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // POST /api/admin/employees  (multipart/form-data)
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateUserDto request)
    {
        try
        {
            var result = await _employeeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { empId = result.EmpId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET /api/admin/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _employeeService.GetAllAsync();
        return Ok(result);
    }

    // DELETE /api/admin/employees/{empId}
    [HttpDelete("{empId}")]
    public async Task<IActionResult> Delete(long empId)
    {
        var success = await _employeeService.DeleteAsync(empId);
        if (!success) return NotFound(new { message = $"Employee {empId} not found" });
        return NoContent(); // 204 — ลบสำเร็จ ไม่มี body
    }
}
