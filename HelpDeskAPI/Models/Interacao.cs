namespace HelpDeskAPI.Models;

public class Interacao
{
    public int Id { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataRegistro { get; set; }
    public bool Interna { get; set; }
    public int ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }
    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }
}
