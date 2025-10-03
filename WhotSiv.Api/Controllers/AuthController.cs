using Microsoft.AspNetCore.Mvc;
using WhotSiv.Api.DTOs;
using WhotSiv.Api.Services;

namespace WhotSiv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{

    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginRequest loginRequest
    )
    {
        return await _authService.DoLogin(loginRequest);
    }

}