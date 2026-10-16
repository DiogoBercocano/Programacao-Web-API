using System.ComponentModel.DataAnnotations;

namespace Acervo.DTOS.Autor
{
    public class AutorCreateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;
    }
}
