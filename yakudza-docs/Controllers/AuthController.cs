using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yakudza_docs.Data;
using yakudza_docs.DTOs;
using yakudza_docs.Models;
using yakudza_docs.Services;

namespace yakudza_docs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid login or password" });
        }

        var isPasswordValid = PasswordHasher.VerifyPasswordHash(
            request.Password,
            user.PasswordHash,
            user.PasswordSalt
        );

        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid login or password" });
        }

        var token = _jwtService.GenerateToken(user, user.Role.Name);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Login = user.Login,
            Role = user.Role.Name
        });
    }

    [HttpPost("init-admin")]
    public async Task<ActionResult<LoginResponseDto>> InitAdmin([FromBody] InitAdminRequestDto request)
    {
        // Check if any admin already exists
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            return StatusCode(500, new { message = "Admin role not found in database" });
        }

        var adminExists = await _context.Users.AnyAsync(u => u.RoleId == adminRole.Id);
        if (adminExists)
        {
            return BadRequest(new { message = "Admin already exists. Use login instead." });
        }

        // Check if login is already taken
        var loginExists = await _context.Users.AnyAsync(u => u.Login == request.Login);
        if (loginExists)
        {
            return BadRequest(new { message = "Login already taken" });
        }

        // Create password hash and salt
        PasswordHasher.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

        // Create admin user
        var adminUser = new User
        {
            Login = request.Login,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = adminRole.Id
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        // Load the role for token generation
        await _context.Entry(adminUser).Reference(u => u.Role).LoadAsync();

        var token = _jwtService.GenerateToken(adminUser, adminUser.Role.Name);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Login = adminUser.Login,
            Role = adminUser.Role.Name
        });
    }

    [HttpGet("check-admin")]
    public async Task<ActionResult<bool>> CheckAdminExists()
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            return false;
        }

        var adminExists = await _context.Users.AnyAsync(u => u.RoleId == adminRole.Id);
        return Ok(adminExists);
    }
}
