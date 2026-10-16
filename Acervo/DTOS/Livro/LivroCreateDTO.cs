

using System.ComponentModel.DataAnnotations;

namespace Acervo.DTOS.Livro
{
    public class LivroCreateDTO
    {
        [Required(ErrorMessage = "O campo Titulo é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo AutorId é obrigatório.")]
        public int AutorId { get; set; }         
        [Range(1, 2500, ErrorMessage = "O ano de publicação deve estar entre 1 e 2500.")]
        public int AnoPublicacao { get; set; }
        public List<int> CategoriaIds { get; set; } = null!;
    }
}
