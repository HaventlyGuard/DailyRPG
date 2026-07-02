using System.Security.Claims;
using character_service.Core.DTOs;
using character_service.Features.Characteristics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace character_service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacteristicsController : ControllerBase
{
    private readonly ICharacteristicService _service;
    public CharacteristicsController(ICharacteristicService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacteristicRequest request)
        => Ok(await _service.CreateAsync(GetUserId(), request));

    [HttpGet]
    public async Task<IActionResult> GetMy()
        => Ok(await _service.GetByUserAsync(GetUserId()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id, GetUserId()));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCharacteristicRequest request)
        => Ok(await _service.UpdateAsync(id, GetUserId(), request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }

    [HttpPost("{id}/add-experience")]
    public async Task<IActionResult> AddExperience(Guid id, [FromQuery] int amount)
    {
        await _service.AddExperienceAsync(id, GetUserId(), amount);
        return Ok();
    }

    [HttpGet("rank")]
    public async Task<IActionResult> GetMyRank()
        => Ok(await _service.GetUserRankAsync(GetUserId()));

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}