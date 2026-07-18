using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inventory_service.Core.DTOs;
using inventory_service.Features.Inventory;

namespace inventory_service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    public InventoryController(IInventoryService service) => _service = service;

    /// <summary>
    /// Получить весь инвентарь
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetInventory()
        => Ok(await _service.GetInventoryAsync(GetUserId()));

    /// <summary>
    /// Получить экипированные предметы
    /// </summary>
    [HttpGet("equipped")]
    public async Task<IActionResult> GetEquipped()
        => Ok(await _service.GetEquippedItemsAsync(GetUserId()));

    /// <summary>
    /// Экипировать предмет в слот
    /// </summary>
    [HttpPost("{itemId}/equip")]
    public async Task<IActionResult> EquipItem(Guid itemId, [FromBody] EquipRequest request)
        => Ok(await _service.EquipItemAsync(GetUserId(), itemId, request.Slot));

    /// <summary>
    /// Снять предмет
    /// </summary>
    [HttpPost("{itemId}/unequip")]
    public async Task<IActionResult> UnequipItem(Guid itemId)
        => Ok(await _service.UnequipItemAsync(GetUserId(), itemId));

    /// <summary>
    /// Использовать расходуемый предмет
    /// </summary>
    [HttpPost("{itemId}/use")]
    public async Task<IActionResult> UseItem(Guid itemId)
    {
        await _service.UseItemAsync(GetUserId(), itemId);
        return Ok(new { message = "Item used" });
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
        => Ok(new { Status = "Healthy", Service = "Inventory Service" });

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}