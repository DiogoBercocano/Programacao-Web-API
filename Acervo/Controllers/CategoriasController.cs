using Acervo.DTOS.Categoria;
using Acervo.DTOS.Validation;
using Acervo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Acervo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriasController(ICategoriaService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.ListarAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _service.BuscarPorIdAsync(id);

            if (categoria == null)
            {
                return NotFound(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Mensagem = "Categoria não encontrada"
                });
            }

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoriaCreateDTO dto)
        {
            try
            {
                var categoria = await _service.CadastrarAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
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
        public async Task<IActionResult> Alterar(int id, [FromBody] CategoriaCreateDTO dto)
        {
            try
            {
                var categoria = await _service.AlterarAsync(id, dto);

                if (categoria == null)
                {
                    return NotFound(new ErroResponseDTO
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Mensagem = "Categoria não encontrada"
                    });
                }

                return Ok(categoria);
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
            var excluido = await _service.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Mensagem = "Categoria não encontrada"
                });
            }

            return NoContent();
        }
    }
}
