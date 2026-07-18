using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using task_service.Core.DTOs;
using task_service.Features.Tasks;

namespace task_service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    public TasksController(ITaskService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        => Ok(await _service.CreateAsync(GetUserId(), request));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id, GetUserId()));

    [HttpGet("date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        => Ok(await _service.GetByDateAsync(GetUserId(), date));

    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(await _service.GetByDateRangeAsync(GetUserId(), from, to));

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
        => Ok(await _service.GetDashboardAsync(GetUserId()));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
        => Ok(await _service.UpdateAsync(id, GetUserId(), request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
        => Ok(await _service.CompleteAsync(id, GetUserId()));

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
        => Ok(new { Status = "Healthy", Service = "Task Service" });

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}