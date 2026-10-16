using System.ComponentModel.DataAnnotations;

namespace Acervo.DTOS.Categoria
{
    public class CategoriaCreateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;
    }
}
