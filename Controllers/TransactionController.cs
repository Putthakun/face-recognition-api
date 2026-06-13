using face_recognition_api.Data;
using face_recognition_api.DTOs;
using face_recognition_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/transactions — called by face-recognition-server (internal, no auth)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto request)
    {
        var transaction = new Transaction
        {
            EmpId    = request.EmpId,
            CameraId = request.CameraId,
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        return Ok(new { transaction.TransactionId, transaction.CreatedAt });
    }

    // GET /api/transactions — Admin and Supervisor
    [HttpGet]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool sortDesc = true)
    {
        var base_query = _db.Transactions
            .Include(t => t.Employee)
            .Include(t => t.Camera);

        var query = sortDesc
            ? base_query.OrderByDescending(t => t.CreatedAt)
            : base_query.OrderBy(t => t.CreatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionResponseDto
            {
                TransactionId  = t.TransactionId,
                EmpId          = t.EmpId,
                EmpName        = t.Employee != null ? t.Employee.Name : null,
                CameraId       = t.CameraId,
                CameraLocation = t.Camera != null ? t.Camera.Location : null,
                CreatedAt      = t.CreatedAt,
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }
}
