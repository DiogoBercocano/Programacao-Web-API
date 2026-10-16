using Acervo.DTOS.Autor;
using Acervo.DTOS.Categoria;

namespace Acervo.DTOS.Livro
{
    public class LivroResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public AutorDTO? Autor {  get; set; }
        public int AnoPublicacao { get; set; }
        public List<CategoriaDTO> Categorias { get; set; } = null!; 
    }
}
