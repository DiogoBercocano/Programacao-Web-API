using Acervo.Data;
using Acervo.DTOS.Categoria;
using Acervo.Models;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context) => _context = context;

        public async Task<List<CategoriaDTO>> ListarAsync()
        {
            return await _context.Categorias
                .OrderBy(c => c.Nome)
                .Select(c => new CategoriaDTO { Id = c.Id, Nome = c.Nome })
                .ToListAsync();
        }

        public async Task<CategoriaDTO?> BuscarPorIdAsync(int id)
        {
            return await _context.Categorias
                .Where(c => c.Id == id)
                .Select(c => new CategoriaDTO { Id = c.Id, Nome = c.Nome })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoriaDTO> CadastrarAsync(CategoriaCreateDTO dto)
        {
            var existe = await _context.Categorias.AnyAsync(c => c.Nome == dto.Nome);

            if (existe)
                throw new InvalidOperationException("Já existe uma categoria com este nome.");

            var categoria = new Categoria { Nome = dto.Nome };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome };
        }

        public async Task<CategoriaDTO?> AlterarAsync(int id, CategoriaCreateDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return null;

            var existe = await _context.Categorias
                .AnyAsync(c => c.Nome == dto.Nome && c.Id != id);

            if (existe)
                throw new InvalidOperationException("Já existe uma categoria com este nome.");

            categoria.Nome = dto.Nome;
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome };
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
