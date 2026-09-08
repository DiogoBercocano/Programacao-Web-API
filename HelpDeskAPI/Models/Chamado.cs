using HelpDeskAPI.Models.Enums;

namespace HelpDeskAPI.Models;

public class Chamado
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public PrioridadeChamado Prioridade { get; set; }
    public StatusChamado Status { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public int SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }
    public int? TecnicoId { get; set; }
    public Usuario? Tecnico { get; set; }
    public List<Interacao> Interacoes { get; set; } = new();
}
