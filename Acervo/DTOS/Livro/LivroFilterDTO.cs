namespace Acervo.DTOS.Livro
{
    public class LivroFilterDTO
    {
        public string? Titulo { get; set; }
        public int? AnoInicio { get; set; }
        public int? AnoFim { get; set; }
        public int? AutorId { get; set; }
        public string? Autor {  get; set; }
        public int? Categoria {  get; set; }
        public string? OrdenarPor {  get; set; }
    }
}
