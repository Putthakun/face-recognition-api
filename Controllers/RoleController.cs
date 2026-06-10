using face_recognition_api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoleController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/role — get all roles for dropdown
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _db.Roles
            .Select(r => new { r.RoleId, r.RoleName, r.IsSystem })
            .ToListAsync();

        return Ok(roles);
    }
}
