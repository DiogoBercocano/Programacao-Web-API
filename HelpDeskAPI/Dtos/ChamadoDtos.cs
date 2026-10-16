using System.ComponentModel.DataAnnotations;
using HelpDeskAPI.Models.Enums;

namespace HelpDeskAPI.Dtos;

public class ChamadoCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(60, ErrorMessage = "A categoria deve ter no máximo 60 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A prioridade é obrigatória.")]
    [EnumDataType(typeof(PrioridadeChamado), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeChamado Prioridade { get; set; }

    [Required(ErrorMessage = "O solicitante é obrigatório.")]
    public int SolicitanteId { get; set; }

    public int? TecnicoId { get; set; }
}

public class ChamadoUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(60, ErrorMessage = "A categoria deve ter no máximo 60 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A prioridade é obrigatória.")]
    [EnumDataType(typeof(PrioridadeChamado), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeChamado Prioridade { get; set; }

    [Required(ErrorMessage = "O status é obrigatório.")]
    [EnumDataType(typeof(StatusChamado), ErrorMessage = "Status inválido.")]
    public StatusChamado Status { get; set; }

    [Required(ErrorMessage = "O solicitante é obrigatório.")]
    public int SolicitanteId { get; set; }

    public int? TecnicoId { get; set; }

    public DateTime? DataFechamento { get; set; }
}
