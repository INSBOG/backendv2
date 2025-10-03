using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WhotSiv.Api.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Genera un token JWT para un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="email">Email del usuario</param>
    /// <param name="additionalClaims">Claims adicionales opcionales</param>
    /// <returns>Token JWT como string</returns>
    public string GenerateToken(int userId, string email, Dictionary<string, string>? additionalClaims = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey no está configurado")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"{userId}"),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar claims adicionales si existen
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Resultado de la validación con información del token</returns>
    public TokenValidationResult ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey no está configurado")));

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Sin tolerancia de tiempo
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return new TokenValidationResult
            {
                IsValid = true,
                IsExpired = false,
                UserId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
                Email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value,
                Claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value),
                ExpirationDate = jwtToken.ValidTo
            };
        }
        catch (SecurityTokenExpiredException)
        {
            return new TokenValidationResult
            {
                IsValid = false,
                IsExpired = true,
                Error = "El token ha expirado"
            };
        }
        catch (Exception ex)
        {
            return new TokenValidationResult
            {
                IsValid = false,
                IsExpired = false,
                Error = $"Token inválido: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Verifica si un token está expirado sin validar su firma
    /// </summary>
    /// <param name="token">Token a verificar</param>
    /// <returns>True si el token está expirado</returns>
    public bool IsTokenExpired(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            return true; // Si no se puede leer, lo consideramos expirado
        }

        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.ValidTo < DateTime.UtcNow;
    }
}

/// <summary>
/// Resultado de la validación de un token JWT
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public bool IsExpired { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Error { get; set; }
}
