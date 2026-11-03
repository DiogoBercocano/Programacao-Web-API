using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskAPI.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResponseDto GerarToken(Usuario usuario)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A chave JWT não foi configurada.");
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("O Issuer JWT não foi configurado.");
        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("A Audience JWT não foi configurada.");
        var minutos = _configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;

        var expiracao = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiracao,
            signingCredentials: credentials);

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracao = expiracao,
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Perfil = usuario.Perfil.ToString()
        };
    }
}
