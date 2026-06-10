using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AdminController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    // POST /api/admin/users — create a new employee account
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var response = await _authService.CreateUserAsync(request);
        return Ok(response);
    }

    // GET /api/admin/users — list all employees with credentials
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var employees = await _userRepository.GetAllAsync();

        var result = employees.Select(e => new
        {
            e.EmpId,
            e.Name,
            Role = e.Credential?.Role,
            IsActive = e.Credential?.IsActive,
            CreatedAt = e.Credential?.CreatedAt
        });

        return Ok(result);
    }
}
