using Acervo.DTOS.Autor;
using Acervo.DTOS.Validation;
using Acervo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Acervo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _service;

        public AutoresController(IAutorService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.ListarAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var autor = await _service.BuscarPorIdAsync(id);

            if (autor == null)
            {
                return NotFound(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Mensagem = "Autor não encontrado"
                });
            }

            return Ok(autor);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AutorCreateDTO dto)
        {
            try
            {
                var autor = await _service.CadastrarAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = autor.Id }, autor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Mensagem = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Alterar(int id, [FromBody] AutorCreateDTO dto)
        {
            try
            {
                var autor = await _service.AlterarAsync(id, dto);

                if (autor == null)
                {
                    return NotFound(new ErroResponseDTO
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Mensagem = "Autor não encontrado"
                    });
                }

                return Ok(autor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Mensagem = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var excluido = await _service.ExcluirAsync(id);

                if (!excluido)
                {
                    return NotFound(new ErroResponseDTO
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Mensagem = "Autor não encontrado"
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Mensagem = ex.Message
                });
            }
        }
    }
}
