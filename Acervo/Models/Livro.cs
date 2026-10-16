namespace Acervo.Models
{
    public class Livro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; } = null!;
        public ICollection<Categoria> Categorias { get; set; } = null!;
    }
}
