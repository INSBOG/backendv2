using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhotSiv.Api.DTOs;
using WhotSiv.Api.Data;
using Azure;

namespace WhotSiv.Api.Services;

public class AuthService
{

    private readonly AppDbContext _dbContext;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public AuthService(
        AppDbContext dbContext,
        PasswordService passwordService,
        JwtService jwtService
    )
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<IActionResult> DoLogin(UserLoginRequest loginRequest)
    {
        var user = await this._dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Username || u.Username == loginRequest.Username);
        if (user is null)
        {
            return new NotFoundObjectResult(new { message = "Usuario no encontrado" });
        }

        if (!_passwordService.VerifyPassword(loginRequest.Password, user.Password))
        {
            return new BadRequestObjectResult(new { message = "La contraseña es incorrecta" });
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email);

        user.Password = "";

        return new OkObjectResult(new
        {
            data = user
        });
    }
}