using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartStock.Domain.Identity;
using SmartStock.WebAPI.Services;

namespace SmartStock.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public record LoginRequest(string UserName, string Password);
    public record AuthResponse(string Token);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null) return Unauthorized();

        var res = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!res.Succeeded) return Unauthorized();

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(new AuthResponse(token));
    }

    public record CreateSellerRequest(string UserName, string Password);

    [HttpPost("sellers")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateSeller(CreateSellerRequest request)
    {
        var existing = await _userManager.FindByNameAsync(request.UserName);
        if (existing is not null)
            return Conflict("UserName already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName.Trim()
        };

        var create = await _userManager.CreateAsync(user, request.Password);
        if (!create.Succeeded)
            return BadRequest(create.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, Roles.Sotuvchi);
        return CreatedAtAction(nameof(CreateSeller), new { id = user.Id }, new { user.Id, user.UserName });
    }
}

