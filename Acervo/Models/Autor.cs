namespace Acervo.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public ICollection<Livro>? Livros { get; set; } 
    }
}
