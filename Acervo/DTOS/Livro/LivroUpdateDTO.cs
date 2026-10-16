using System.ComponentModel.DataAnnotations;

namespace Acervo.DTOS.Livro
{
    public class LivroUpdateDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public int AutorId { get; set; }
        [Range(1, 2500, ErrorMessage = "O ano de publicação deve estar entre 1 e 2500.")]
        public int AnoPublicacao { get; set; }
        public List<int> CategoriaIds { get; set; } = null!;
    }
}
