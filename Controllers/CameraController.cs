using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace face_recognition_api.Controllers;

[ApiController]
[Route("api/admin/cameras")]
[Authorize(Roles = "Admin")]
public class CameraController : ControllerBase
{
    private readonly ICameraService _cameraService;

    public CameraController(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }

    // POST /api/admin/cameras
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCameraDto request)
    {
        try
        {
            var result = await _cameraService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { cameraId = result.CameraId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET /api/admin/cameras
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _cameraService.GetAllAsync();
        return Ok(result);
    }

    // PUT /api/admin/cameras/{cameraId}
    [HttpPut("{cameraId}")]
    public async Task<IActionResult> Update(long cameraId, [FromBody] UpdateCameraDto request)
    {
        var result = await _cameraService.UpdateAsync(cameraId, request);
        if (result == null) return NotFound(new { message = $"Camera {cameraId} not found" });
        return Ok(result);
    }

    // DELETE /api/admin/cameras/{cameraId}
    [HttpDelete("{cameraId}")]
    public async Task<IActionResult> Delete(long cameraId)
    {
        var success = await _cameraService.DeleteAsync(cameraId);
        if (!success) return NotFound(new { message = $"Camera {cameraId} not found" });
        return NoContent();
    }
}
