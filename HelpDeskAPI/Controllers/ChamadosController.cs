using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Chamado>> GetTodos()
    {
        return Ok(RepositorioEmMemoria.Chamados);
    }

    [HttpGet("{id}")]
    public ActionResult<Chamado> GetPorId(int id)
    {
        var chamado = RepositorioEmMemoria.Chamados.FirstOrDefault(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        return Ok(chamado);
    }

    [HttpGet("{id}/interacoes")]
    public ActionResult<IEnumerable<Interacao>> GetInteracoes(int id)
    {
        var chamado = RepositorioEmMemoria.Chamados.FirstOrDefault(c => c.Id == id);

        if (chamado is null)
        {
            return NotFound(new { mensagem = $"Chamado com Id {id} nao foi encontrado." });
        }

        var interacoesOrdenadas = chamado.Interacoes
            .OrderBy(i => i.DataRegistro)
            .ToList();

        return Ok(interacoesOrdenadas);
    }
}
