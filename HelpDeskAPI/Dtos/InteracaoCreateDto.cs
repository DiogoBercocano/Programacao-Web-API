using System.ComponentModel.DataAnnotations;

namespace HelpDeskAPI.Dtos;

public class InteracaoCreateDto
{
    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "A mensagem deve ter entre 3 e 1000 caracteres.")]
    public string Mensagem { get; set; } = string.Empty;

    public bool Interna { get; set; }

    [Required(ErrorMessage = "O autor é obrigatório.")]
    public int AutorId { get; set; }
}
