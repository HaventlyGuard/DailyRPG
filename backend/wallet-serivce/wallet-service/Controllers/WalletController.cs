using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wallet_service.Core.DTOs;
using wallet_service.Features.Wallet;

namespace wallet_service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _service;
    public WalletController(IWalletService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetWallet()
        => Ok(await _service.GetOrCreateWalletAsync(GetUserId()));

    [HttpPost("add")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddCurrency([FromBody] AddCurrencyRequest request)
    {
        var currencyType = Enum.Parse<CurrencyType>(request.CurrencyType, true);
        return Ok(await _service.AddCurrencyAsync(GetUserId(), currencyType, request.Amount, request.Source));
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _service.GetTransactionsAsync(GetUserId(), page, pageSize));

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
        => Ok(new { Status = "Healthy", Service = "Wallet Service" });

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!);
    }
}