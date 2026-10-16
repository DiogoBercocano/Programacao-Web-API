using Acervo.DTOS.Livro;

namespace Acervo.Services
{
    public interface ILivroService
    {
        Task<List<LivroResponseDTO>> ListarAsync(LivroFilterDTO filtro);
        Task<LivroResponseDTO?> BuscarPorIdAsync(int id);
        Task<LivroResponseDTO> CadastrarAsync(LivroCreateDTO dto);
        Task<LivroResponseDTO?> AlterarAsync(
            int id, 
            LivroUpdateDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
