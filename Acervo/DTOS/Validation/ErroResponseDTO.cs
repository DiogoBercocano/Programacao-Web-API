namespace Acervo.DTOS.Validation
{
    public class ErroResponseDTO
    {
        public int StatusCode { get; set; }
        public string Mensagem {  get; set; } = string.Empty;
        public Dictionary<string, string[]>? Erros { get; set; }

    }
}
