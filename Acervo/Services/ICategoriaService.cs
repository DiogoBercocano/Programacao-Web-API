using Acervo.DTOS.Categoria;

namespace Acervo.Services
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDTO>> ListarAsync();
        Task<CategoriaDTO?> BuscarPorIdAsync(int id);
        Task<CategoriaDTO> CadastrarAsync(CategoriaCreateDTO dto);
        Task<CategoriaDTO?> AlterarAsync(int id, CategoriaCreateDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
