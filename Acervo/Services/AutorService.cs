using Acervo.Data;
using Acervo.DTOS.Autor;
using Acervo.Models;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Services
{
    public class AutorService : IAutorService
    {
        private readonly AppDbContext _context;

        public AutorService(AppDbContext context) => _context = context;

        public async Task<List<AutorDTO>> ListarAsync()
        {
            return await _context.Autores
                .OrderBy(a => a.Nome)
                .Select(a => new AutorDTO { Id = a.Id, Nome = a.Nome })
                .ToListAsync();
        }

        public async Task<AutorDTO?> BuscarPorIdAsync(int id)
        {
            return await _context.Autores
                .Where(a => a.Id == id)
                .Select(a => new AutorDTO { Id = a.Id, Nome = a.Nome })
                .FirstOrDefaultAsync();
        }

        public async Task<AutorDTO> CadastrarAsync(AutorCreateDTO dto)
        {
            var existe = await _context.Autores.AnyAsync(a => a.Nome == dto.Nome);

            if (existe)
                throw new InvalidOperationException("Já existe um autor com este nome.");

            var autor = new Autor { Nome = dto.Nome };

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return new AutorDTO { Id = autor.Id, Nome = autor.Nome };
        }

        public async Task<AutorDTO?> AlterarAsync(int id, AutorCreateDTO dto)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
                return null;

            var existe = await _context.Autores
                .AnyAsync(a => a.Nome == dto.Nome && a.Id != id);

            if (existe)
                throw new InvalidOperationException("Já existe um autor com este nome.");

            autor.Nome = dto.Nome;
            await _context.SaveChangesAsync();

            return new AutorDTO { Id = autor.Id, Nome = autor.Nome };
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
                return false;

            var possuiLivros = await _context.Livros.AnyAsync(l => l.AutorId == id);

            if (possuiLivros)
                throw new InvalidOperationException(
                    "Não é possível excluir o autor pois existem livros vinculados a ele.");

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
