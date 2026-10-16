using HelpDeskAPI.Data;
using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public UsuariosController(HelpDeskDbContext context)
    {
        _context = context;
    }

    // GET api/usuarios?perfil=Tecnico&ativo=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetTodos([FromQuery] PerfilUsuario? perfil, [FromQuery] bool? ativo)
    {
        var query = _context.Usuarios.AsNoTracking().AsQueryable();

        if (perfil.HasValue)
        {
            query = query.Where(u => u.Perfil == perfil.Value);
        }

        if (ativo.HasValue)
        {
            query = query.Where(u => u.Ativo == ativo.Value);
        }

        var usuarios = await query.OrderBy(u => u.Nome).ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> GetPorId(int id)
    {
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        if (usuario is null)
        {
            return NotFound(new { mensagem = $"Usuario com Id {id} nao foi encontrado." });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Criar(UsuarioCreateDto dto)
    {
        var emailEmUso = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (emailEmUso)
        {
            return BadRequest(new { mensagem = $"Já existe um usuário cadastrado com o e-mail '{dto.Email}'." });
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Setor = dto.Setor,
            Perfil = dto.Perfil,
            Ativo = dto.Ativo
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, UsuarioUpdateDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

        if (usuario is null)
        {
            return NotFound(new { mensagem = $"Usuario com Id {id} nao foi encontrado." });
        }

        var emailEmUso = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id);
        if (emailEmUso)
        {
            return BadRequest(new { mensagem = $"Já existe um usuário cadastrado com o e-mail '{dto.Email}'." });
        }

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.Setor = dto.Setor;
        usuario.Perfil = dto.Perfil;
        usuario.Ativo = dto.Ativo;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

        if (usuario is null)
        {
            return NotFound(new { mensagem = $"Usuario com Id {id} nao foi encontrado." });
        }

        var possuiVinculos = await _context.Chamados.AnyAsync(c => c.SolicitanteId == id || c.TecnicoId == id)
            || await _context.Interacoes.AnyAsync(i => i.AutorId == id);

        if (possuiVinculos)
        {
            return BadRequest(new { mensagem = "Não é possível excluir o usuário pois existem chamados ou interações vinculados a ele." });
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
