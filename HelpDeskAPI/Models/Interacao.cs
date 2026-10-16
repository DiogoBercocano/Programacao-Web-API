using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models;

public class Interacao
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "A mensagem deve ter entre 3 e 1000 caracteres.")]
    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataRegistro { get; set; } = DateTime.Now;

    public bool Interna { get; set; }

    [Required(ErrorMessage = "O chamado é obrigatório.")]
    public int ChamadoId { get; set; }

    [JsonIgnore]
    public Chamado? Chamado { get; set; }

    [Required(ErrorMessage = "O autor é obrigatório.")]
    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }
}
