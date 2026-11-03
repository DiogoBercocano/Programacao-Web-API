using HelpDeskAPI.Data;
using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;
using System.Security.Claims;
using HelpDeskAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public ChamadosController(HelpDeskDbContext context)
    {
        _context = context;
    }

    private int UsuarioLogadoId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool EhSolicitante => User.IsInRole(Perfis.Solicitante);

    private bool EhAdministrador => User.IsInRole(Perfis.Administrador);

    // GET api/chamados?status=Aberto&prioridade=Alta&categoria=Hardware&solicitanteId=1&tecnicoId=3
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chamado>>> GetTodos(
        [FromQuery] StatusChamado? status,
        [FromQuery] PrioridadeChamado? prioridade,
        [FromQuery] string? categoria,
        [FromQuery] int? solicitanteId,
        [FromQuery] int? tecnicoId)
    {
        var query = _context.Chamados
            .AsNoTracking()
            .Include(c => c.Solicitante)
            .Include(c => c.Tecnico)
            .AsQueryable();

        if (EhSolicitante)
        {
            solicitanteId = UsuarioLogadoId;
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (prioridade.HasValue)
        {
            query = query.Where(c => c.Prioridade == prioridade.Value);
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(c => c.Categoria.Contains(categoria));
        }

        if (solicitanteId.HasValue)
        {
            query = query.Where(c => c.SolicitanteId == solicitanteId.Value);
        }

        if (tecnicoId.HasValue)
        {
            query = query.Where(c => c.TecnicoId == tecnicoId.Value);
        }

        var chamados = await query.OrderByDescending(c => c.DataAbertura).ToListAsync();

        return Ok(chamados);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Chamado>> GetPorId(int id)
    {
        var chamado = await _context.Chamados
            .AsNoTracking()
            .Include(c => c.Solicitante)
            .Include(c => c.Tecnico)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} não foi encontrado." });
        }

        if (EhSolicitante && chamado.SolicitanteId != UsuarioLogadoId)
        {
            return Forbid();
        }

        return Ok(chamado);
    }

    [HttpGet("{id:int}/interacoes")]
    public async Task<ActionResult<IEnumerable<Interacao>>> GetInteracoes(int id)
    {
        var chamado = await _context.Chamados.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} não foi encontrado." });
        }

        if (EhSolicitante && chamado.SolicitanteId != UsuarioLogadoId)
        {
            return Forbid();
        }

        var interacoes = await _context.Interacoes
            .AsNoTracking()
            .Include(i => i.Autor)
            .Where(i => i.ChamadoId == id && (!EhSolicitante || !i.Interna))
            .OrderBy(i => i.DataRegistro)
            .ToListAsync();

        return Ok(interacoes);
    }

    [HttpPost]
    public async Task<ActionResult<Chamado>> Criar(ChamadoCreateDto dto)
    {
        if (EhSolicitante)
        {
            // Solicitante só abre chamados em seu nome e não escolhe o técnico.
            dto.SolicitanteId = UsuarioLogadoId;
            dto.TecnicoId = null;
        }

        var solicitante = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId);
        if (solicitante is null)
        {
            return BadRequest(new { mensagem = $"Solicitante com Id {dto.SolicitanteId} não foi encontrado." });
        }

        if (dto.TecnicoId.HasValue)
        {
            var tecnico = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.TecnicoId.Value);
            if (tecnico is null)
            {
                return BadRequest(new { mensagem = $"Técnico com Id {dto.TecnicoId.Value} não foi encontrado." });
            }

            if (tecnico.Perfil != PerfilUsuario.Tecnico)
            {
                return BadRequest(new { mensagem = "O usuário informado como técnico não possui perfil Técnico." });
            }

            if (!tecnico.Ativo)
            {
                return BadRequest(new { mensagem = "O técnico informado está inativo." });
            }
        }

        var chamado = new Chamado
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Categoria = dto.Categoria,
            Prioridade = dto.Prioridade,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.UtcNow,
            DataFechamento = null,
            SolicitanteId = dto.SolicitanteId,
            TecnicoId = dto.TecnicoId
        };

        _context.Chamados.Add(chamado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPorId), new { id = chamado.Id }, chamado);
    }

    [Authorize(Roles = Perfis.TecnicoOuAdministrador)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ChamadoUpdateDto dto)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} não foi encontrado." });
        }

        var solicitante = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId);
        if (solicitante is null)
        {
            return BadRequest(new { mensagem = $"Solicitante com Id {dto.SolicitanteId} não foi encontrado." });
        }

        if (dto.TecnicoId.HasValue)
        {
            var tecnico = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.TecnicoId.Value);
            if (tecnico is null)
            {
                return BadRequest(new { mensagem = $"Técnico com Id {dto.TecnicoId.Value} não foi encontrado." });
            }

            if (tecnico.Perfil != PerfilUsuario.Tecnico)
            {
                return BadRequest(new { mensagem = "O usuário informado como técnico não possui perfil Técnico." });
            }

            if (!tecnico.Ativo)
            {
                return BadRequest(new { mensagem = "O técnico informado está inativo." });
            }
        }

        var statusDeFechamento = dto.Status is StatusChamado.Resolvido or StatusChamado.Fechado or StatusChamado.Cancelado;
        if (!statusDeFechamento && dto.DataFechamento.HasValue)
        {
            return BadRequest(new { mensagem = "A data de fechamento só pode ser informada quando o status for Resolvido, Fechado ou Cancelado." });
        }

        chamado.Titulo = dto.Titulo;
        chamado.Descricao = dto.Descricao;
        chamado.Categoria = dto.Categoria;
        chamado.Prioridade = dto.Prioridade;
        chamado.Status = dto.Status;
        chamado.SolicitanteId = dto.SolicitanteId;
        chamado.TecnicoId = dto.TecnicoId;
        chamado.DataFechamento = statusDeFechamento ? dto.DataFechamento ?? chamado.DataFechamento ?? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = Perfis.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} não foi encontrado." });
        }

        _context.Chamados.Remove(chamado);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/interacoes")]
    public async Task<ActionResult<Interacao>> CriarInteracao(int id, InteracaoCreateDto dto)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);
        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} não foi encontrado." });
        }

        if (chamado.Status is StatusChamado.Fechado or StatusChamado.Cancelado)
        {
            return BadRequest(new { mensagem = "Não é possível registrar interações em chamados fechados ou cancelados." });
        }

        if (EhSolicitante)
        {
            if (chamado.SolicitanteId != UsuarioLogadoId)
            {
                return Forbid();
            }

            if (dto.Interna)
            {
                return BadRequest(new { mensagem = "Solicitantes não podem registrar interações internas." });
            }
        }

        var interacao = new Interacao
        {
            Mensagem = dto.Mensagem,
            Interna = dto.Interna,
            DataRegistro = DateTime.UtcNow,
            ChamadoId = id,
            AutorId = UsuarioLogadoId
        };

        _context.Interacoes.Add(interacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInteracoes), new { id }, interacao);
    }

    [Authorize(Roles = Perfis.Administrador)]
    [HttpDelete("{id:int}/interacoes/{interacaoId:int}")]
    public async Task<IActionResult> RemoverInteracao(int id, int interacaoId)
    {
        var interacao = await _context.Interacoes.FirstOrDefaultAsync(i => i.Id == interacaoId && i.ChamadoId == id);

        if (interacao is null)
        {
            return NotFound(new { mensagem = $"Interação com Id {interacaoId} não foi encontrada para o chamado {id}." });
        }

        _context.Interacoes.Remove(interacao);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
