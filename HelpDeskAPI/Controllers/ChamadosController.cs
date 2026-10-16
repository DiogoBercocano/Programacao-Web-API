using HelpDeskAPI.Data;
using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public ChamadosController(HelpDeskDbContext context)
    {
        _context = context;
    }

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
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        return Ok(chamado);
    }

    [HttpGet("{id:int}/interacoes")]
    public async Task<ActionResult<IEnumerable<Interacao>>> GetInteracoes(int id)
    {
        var chamadoExiste = await _context.Chamados.AnyAsync(c => c.Id == id);

        if (!chamadoExiste)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        var interacoes = await _context.Interacoes
            .AsNoTracking()
            .Include(i => i.Autor)
            .Where(i => i.ChamadoId == id)
            .OrderBy(i => i.DataRegistro)
            .ToListAsync();

        return Ok(interacoes);
    }

    [HttpPost]
    public async Task<ActionResult<Chamado>> Criar(ChamadoCreateDto dto)
    {
        var solicitante = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId);
        if (solicitante is null)
        {
            return BadRequest(new { mensagem = $"Solicitante com Id {dto.SolicitanteId} nao foi encontrado." });
        }

        if (dto.TecnicoId.HasValue)
        {
            var tecnico = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.TecnicoId.Value);
            if (tecnico is null)
            {
                return BadRequest(new { mensagem = $"Técnico com Id {dto.TecnicoId.Value} nao foi encontrado." });
            }

            if (tecnico.Perfil != PerfilUsuario.Tecnico)
            {
                return BadRequest(new { mensagem = "O usuário informado como técnico não possui perfil Técnico." });
            }
        }

        var chamado = new Chamado
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Categoria = dto.Categoria,
            Prioridade = dto.Prioridade,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.Now,
            DataFechamento = null,
            SolicitanteId = dto.SolicitanteId,
            TecnicoId = dto.TecnicoId
        };

        _context.Chamados.Add(chamado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPorId), new { id = chamado.Id }, chamado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ChamadoUpdateDto dto)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        var solicitante = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId);
        if (solicitante is null)
        {
            return BadRequest(new { mensagem = $"Solicitante com Id {dto.SolicitanteId} nao foi encontrado." });
        }

        if (dto.TecnicoId.HasValue)
        {
            var tecnico = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.TecnicoId.Value);
            if (tecnico is null)
            {
                return BadRequest(new { mensagem = $"Técnico com Id {dto.TecnicoId.Value} nao foi encontrado." });
            }

            if (tecnico.Perfil != PerfilUsuario.Tecnico)
            {
                return BadRequest(new { mensagem = "O usuário informado como técnico não possui perfil Técnico." });
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
        chamado.DataFechamento = statusDeFechamento ? dto.DataFechamento ?? chamado.DataFechamento ?? DateTime.Now : null;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
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
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        var autor = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.AutorId);
        if (autor is null)
        {
            return BadRequest(new { mensagem = $"Autor com Id {dto.AutorId} nao foi encontrado." });
        }

        var interacao = new Interacao
        {
            Mensagem = dto.Mensagem,
            Interna = dto.Interna,
            DataRegistro = DateTime.Now,
            ChamadoId = id,
            AutorId = dto.AutorId
        };

        _context.Interacoes.Add(interacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInteracoes), new { id }, interacao);
    }

    [HttpDelete("{id:int}/interacoes/{interacaoId:int}")]
    public async Task<IActionResult> RemoverInteracao(int id, int interacaoId)
    {
        var interacao = await _context.Interacoes.FirstOrDefaultAsync(i => i.Id == interacaoId && i.ChamadoId == id);

        if (interacao is null)
        {
            return NotFound(new { mensagem = $"Interação com Id {interacaoId} nao foi encontrada para o chamado {id}." });
        }

        _context.Interacoes.Remove(interacao);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
