using System.Security.Claims;
using HelpDeskAPI.Data;
using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;
using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HelpDeskDbContext _context;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly ITokenService _tokenService;

    public AuthController(HelpDeskDbContext context, IPasswordHasher<Usuario> hasher, ITokenService tokenService)
    {
        _context = context;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // Mesma resposta para e-mail inexistente, senha errada ou usuário inativo.
        if (usuario is null || !usuario.Ativo ||
            _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { mensagem = "Credenciais inválidas." });
        }

        return Ok(_tokenService.GerarToken(usuario));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            nome = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email),
            perfil = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
