using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Controllers;
using TodoApp.Application.Errors;
using TodoApp.Domain.IdentityEntities;
using TodoApp.Infrastructure.Identity;

namespace TodoApp.API.Controllers;

public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator) : ApiBaseController
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto model)
    {
        if (await _userManager.FindByNameAsync(model.UserName) != null)
        {
            return BadRequest(new ApiError(400, "Username already exists", new Dictionary<string, string[]>
        {
            { "UserName", new[] { "This username is already taken." } }
        }));
        }

        if (await _userManager.FindByEmailAsync(model.Email) != null)
        {
            return BadRequest(new ApiError(400, "Email already exists", new Dictionary<string, string[]>
        {
            { "Email", new[] { "This email is already in use." } }
        }));
        }

        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray()
                );

            return BadRequest(new ApiError(400, "Registration failed", errors));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Ok(new AuthResponse(token));
    }


    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new ApiError(401, "Authentication failed", new Dictionary<string, string[]>
            {
                ["Credentials"] = new[] { "Invalid email or password" }
            }));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new ApiError(401, "Authentication failed", new Dictionary<string, string[]>
            {
                ["Credentials"] = new[] { "Invalid email or password" }
            }));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return Ok(new AuthResponse(token));
    }


    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var name = User.Identity?.Name;

        var user = await _userManager.FindByNameAsync(name!);
        if (user == null)
        {
            return NotFound(new ApiError(404, "User not found"));
        }

        return Ok(new UserResponse(user.Id, user.Email!, user.UserName!));
    }


    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken()
    {
        var name = User.Identity?.Name;

        var user = await _userManager.FindByNameAsync(name!);
        if (user == null)
        {
            return Unauthorized(new ApiError(401, "Invalid user"));
        }

        var newToken = _jwtTokenGenerator.GenerateToken(user);
        return Ok(new AuthResponse(newToken));
    }

}
