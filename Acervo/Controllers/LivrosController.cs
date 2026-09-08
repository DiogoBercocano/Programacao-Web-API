using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Acervo.Models;

namespace Acervo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private static List<Livro> _livros = new List<Livro>()
            {
            new Livro { Id = 1, Titulo = "Sr dos Aneis", Autor = "J.R.R Tokien", AnoPublicacao = 1954 },
            new Livro { Id = 2, Titulo = "1984", Autor = "George Orwell", AnoPublicacao = 1949 },
            new Livro { Id = 3, Titulo = "Livro 3", Autor = "Autor 3", AnoPublicacao = 2022 }
        };

        public IActionResult GetLivros()
        {
            return Ok(_livros);
        }

        [HttpGet("{id}")]
        public IActionResult GetbyId(int id)
        {
            var livro = _livros.FirstOrDefault(l => l.Id == id);
            if (livro == null)
            {
                return NotFound();
            }
            return Ok(livro);
        }
    }
}
