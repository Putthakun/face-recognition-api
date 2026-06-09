using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // All endpoints in this controller require Admin role
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

    // GET /api/admin/users — list all users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();

        // Map to anonymous object to prevent PasswordHash from leaking
        var result = users.Select(u => new
        {
            u.Id,
            u.EmpId,
            u.Role,
            u.CreatedAt
        });

        return Ok(result);
    }
}
