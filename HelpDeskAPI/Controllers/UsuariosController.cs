using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Usuario>> GetTodos()
    {
        return Ok(RepositorioEmMemoria.Usuarios);
    }

    [HttpGet("{id}")]
    public ActionResult<Usuario> GetPorId(int id)
    {
        var usuario = RepositorioEmMemoria.Usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario is null)
        {
            return NotFound(new { mensagem = $"Usuario com Id {id} nao foi encontrado." });
        }

        return Ok(usuario);
    }
}
