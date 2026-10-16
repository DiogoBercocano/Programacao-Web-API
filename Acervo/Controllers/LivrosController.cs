using Acervo.Data;
using Acervo.DTOS.Livro;
using Acervo.DTOS.Validation;
using Acervo.Models;
using Acervo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Acervo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly ILivroService _service;

        public LivrosController (ILivroService service) => _service = service;       

        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] LivroFilterDTO filtro)
        {

            var livros = await _service.ListarAsync(filtro);

            return Ok(livros);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var livro = await _service.BuscarPorIdAsync(id);

            if (livro == null)
            {
                return NotFound(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Mensagem = "ID não encontrado"
                });
            }

            return Ok(livro);
        }



        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LivroCreateDTO livro)
        {
            try
            {
                var livroResponseDTO = await _service.CadastrarAsync(livro);

                return CreatedAtAction
                    (
                        nameof(GetById),
                        new { id = livroResponseDTO.Id },
                        livroResponseDTO
                    );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Mensagem = ex.Message,
                });
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Alterar(int id, [FromBody] LivroUpdateDTO livroAtualizado)
        {
            try
            {
                var livro = await _service.AlterarAsync(id, livroAtualizado);

                if (livro == null)
                {
                    return NotFound(new ErroResponseDTO
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Mensagem = "Livro não encontrado"
                    });
                }

                return Ok(livro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErroResponseDTO
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Mensagem = ex.Message,
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var livro = await _service.ExcluirAsync(id);

            if (!livro)
                return NotFound();

            return NoContent();
        }




    }
}
