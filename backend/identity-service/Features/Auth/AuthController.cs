using identity_service.Core.Constants;
using identity_service.Core.DTOs;
using identity_service.Core.Jwt;
using identity_service.Core.Security;
using identity_service.Features.User;
using identity_service.Features.User.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;

    public AuthController(
        JwtService jwtService, 
        IUserRepository userRepository,
        PasswordHasher passwordHasher)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username))
            return BadRequest(new { error = "Email and username are required" });

        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
            return BadRequest(new { error = "Password must be at least 6 characters" });

        var existingUser = await _userRepository.GetUserByEmail(request.Email);
        if (existingUser != null)
            return Conflict(new { error = "User with this email already exists" });

        var existingUsername = await _userRepository.GetUserByUsername(request.Username);
        if (existingUsername != null)
            return Conflict(new { error = "Username is already taken" });

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            Role = UserRoles.Player,
            IsActive = true
        };

        await _userRepository.CreateUser(user);

        var token = _jwtService.GenerateToken(
            user.Id.ToString(), 
            user.Email, 
            user.Username, 
            user.Role
        );

        var userDto = MapToDto(user);
        return Ok(new AuthResponse(token, userDto));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { error = "Email and password are required" });

        var user = await _userRepository.GetUserByEmail(request.Email);
        if (user == null || !user.IsActive)
            return Unauthorized(new { error = "Invalid credentials" });

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid credentials" });

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateUser(user);

        var token = _jwtService.GenerateToken(
            user.Id.ToString(), 
            user.Email, 
            user.Username, 
            user.Role
        );

        var userDto = MapToDto(user);
        return Ok(new AuthResponse(token, userDto));
    }

    [HttpGet("test-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestToken(
        [FromQuery] string username = "testplayer",
        [FromQuery] string email = "test@example.com",
        [FromQuery] string password = "Test123!",
        [FromQuery] string role = "admin")
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                Role = role,
                IsActive = true
            };
            await _userRepository.CreateUser(user);
        }

        var token = _jwtService.GenerateToken(
            user.Id.ToString(), 
            user.Email, 
            user.Username, 
            user.Role
        );

        return Ok(new { token, user.Id, user.Email, user.Username, user.Role });
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