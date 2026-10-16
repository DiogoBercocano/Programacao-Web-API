using Acervo.DTOS.Autor;

namespace Acervo.Services
{
    public interface IAutorService
    {
        Task<List<AutorDTO>> ListarAsync();
        Task<AutorDTO?> BuscarPorIdAsync(int id);
        Task<AutorDTO> CadastrarAsync(AutorCreateDTO dto);
        Task<AutorDTO?> AlterarAsync(int id, AutorCreateDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
