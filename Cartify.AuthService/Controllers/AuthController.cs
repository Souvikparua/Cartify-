using Cartify.AuthService.Auth;
using Cartify.AuthService.Data;
using Cartify.AuthService.DTOs;
using Cartify.AuthService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;

    public AuthController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingUser = _context.Users
            .FirstOrDefault(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return BadRequest("Email already exists");
        }

        var allowedRoles = new[] { "Customer", "Dealer", "DeliveryPartner" };
        var role = allowedRoles.Contains(request.Role) ? request.Role! : "Customer";

        var otp = JwtHelper.GenerateOtp();
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            IsVerified = false,
            OtpCode = otp,
            OtpExpiry = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // In production the OTP would be emailed/SMS'd; here we return it for the demo.
        return Ok(new
        {
            message = "Registered. Enter the OTP to verify your account.",
            requiresOtp = true,
            devOtp = otp
        });
    }

    // POST /api/auth/verify-otp  — confirm the code from registration
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return NotFound();
        if (user.IsVerified) return Ok(new { message = "Already verified" });
        if (user.OtpCode != request.Code || user.OtpExpiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired OTP.");

        user.IsVerified = true;
        user.OtpCode = null;
        user.OtpExpiry = null;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Account verified. You can sign in now." });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        if (user.IsBlocked)
        {
            return StatusCode(403, "Your account has been blocked. Please contact support.");
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
        {
            return Unauthorized("Invalid credentials");
        }

        if (!user.IsVerified)
        {
            return StatusCode(403, "Please verify your account with the OTP before signing in.");
        }

        return Ok(new
        {
            message = "Login successful",
            token = JwtHelper.GenerateToken(user),
            email = user.Email,
            fullName = user.FullName,
            role = user.Role
        });
    }

    // GET /api/auth/profile/{email}  (identity only; contact/addresses live in UserService)
    [HttpGet("profile/{email}")]
    public async Task<IActionResult> GetProfile(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.FullName, user.Email, user.Role, user.CreatedAt });
    }

    // PUT /api/auth/profile  (update display name)
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = request.FullName;
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.FullName, user.Email, user.Role });
    }

    // PUT /api/auth/change-password
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return NotFound();
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return BadRequest("Current password is incorrect.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 4)
        {
            return BadRequest("New password must be at least 4 characters.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully" });
    }


    // GET /api/auth/users  (admin — list all customers/users)
    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new { u.Id, u.FullName, u.Email, u.Role, u.IsBlocked, u.CreatedAt })
            .ToListAsync();

        return Ok(users);
    }

    // PUT /api/auth/users/5/block  (admin — block or unblock a user)
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id:int}/block")]
    public async Task<IActionResult> SetBlocked(int id, [FromBody] BlockRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Role == "Admin")
        {
            return BadRequest("Admin accounts cannot be blocked.");
        }

        user.IsBlocked = request.Blocked;
        await _context.SaveChangesAsync();
        return Ok(new { user.Id, user.IsBlocked });
    }
}

public class BlockRequest
{
    public bool Blocked { get; set; }
}