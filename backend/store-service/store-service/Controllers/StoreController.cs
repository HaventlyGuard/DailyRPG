using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using store_service.Core.DTOs;
using store_service.Features.Store;

namespace store_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _service;
    public StoreController(IStoreService service) => _service = service;

    /// <summary>
    /// Получить каталог товаров (доступно всем)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCatalog([FromQuery] string? type = null)
        => Ok(await _service.GetCatalogAsync(type));

    /// <summary>
    /// Получить товар по ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetItem(Guid id)
        => Ok(await _service.GetItemAsync(id));

    /// <summary>
    /// Купить товар
    /// </summary>
    [HttpPost("purchase")]
    [Authorize]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        var userId = GetUserId();
        var result = await _service.PurchaseItemAsync(userId, request);
        return Ok(result);
    }

    // ===== Админские ручки =====

    /// <summary>
    /// Создать товар (admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        => Ok(await _service.CreateItemAsync(request));

    /// <summary>
    /// Обновить товар (admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateItemRequest request)
        => Ok(await _service.UpdateItemAsync(id, request));

    /// <summary>
    /// Удалить товар (admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        await _service.DeleteItemAsync(id);
        return NoContent();
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
        => Ok(new { Status = "Healthy", Service = "Store Service" });

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}