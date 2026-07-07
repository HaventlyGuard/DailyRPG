using System.Security.Claims;
using identity_service.Core.DTOs;
using identity_service.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userService.GetOrCreateUser();
        return Ok(MapToDto(user));
    }

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null)
            return NotFound(new { error = "User not found" });

        return Ok(MapToDto(user));
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        var user = await _userService.UpdateProfile(userId, request);
        return Ok(MapToDto(user));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var users = await _userService.GetAllUsers(page, pageSize);
        return Ok(users.Select(MapToDto));
    }

    [HttpPut("{userId}/role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ChangeUserRole(Guid userId, [FromBody] ChangeRoleRequest request)
    {
        var user = await _userService.ChangeUserRole(userId, request.NewRole);
        return Ok(MapToDto(user));
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _userService.DeleteUser(userId);
        return NoContent();
    }

    [HttpGet("stats")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserStats()
    {
        var stats = await _userService.GetUserStats();
        return Ok(stats);
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "Identity Service" });
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userId!);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.AvatarUrl,
            user.CreatedAt,
            user.LastLoginAt,
            user.IsActive,
            user.Role
        );
    }
}