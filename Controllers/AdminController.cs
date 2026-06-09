using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // ← ทุก endpoint ใน Controller นี้ต้องเป็น Admin เท่านั้น
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AdminController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    // POST /api/admin/users — สร้าง account พนักงานใหม่
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var response = await _authService.CreateUserAsync(request);
        return Ok(response);
    }

    // GET /api/admin/users — ดูรายชื่อพนักงานทั้งหมด
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();

        // map เป็น anonymous object เพื่อไม่ให้ PasswordHash หลุดออกไป
        var result = users.Select(u => new
        {
            u.Id,
            u.Email,
            u.Role,
            u.CreatedAt
        });

        return Ok(result);
    }
}
