using Acervo.Data;
using Acervo.DTOS.Autor;
using Acervo.DTOS.Categoria;
using Acervo.DTOS.Livro;
using Acervo.Models;
using Microsoft.EntityFrameworkCore;

namespace Acervo.Services
{
    public class LivroService : ILivroService
    {

        private readonly AppDbContext _context;

        public LivroService(AppDbContext context) => _context = context;

        public async Task<List<LivroResponseDTO>> ListarAsync(LivroFilterDTO filtro)
        {

            var query = _context.Livros.AsQueryable();

            if(!string.IsNullOrWhiteSpace(filtro.Titulo))
            {
                query = query.Where(
                    l => l.Titulo.Contains(filtro.Titulo)
                    );
            }

            if (filtro.AnoInicio.HasValue)
            {
                query = query.Where(
                    l => l.AnoPublicacao >= filtro.AnoInicio.Value
                    );
            }

            if (filtro.AnoFim.HasValue)
            {
                query = query.Where(
                    l => l.AnoPublicacao <= filtro.AnoFim.Value
                    );
            }

            if (!string.IsNullOrWhiteSpace(filtro.Autor))
            {
                query = query.Where(
                    l => l.Autor.Nome.Contains(filtro.Autor)
                    );
            }

            if (filtro.AutorId.HasValue) 
            {
                query = query.Where(
                    l => l.AutorId == filtro.AutorId.Value
                    );
            }

            if (filtro.Categoria.HasValue)
            {
                query = query.Where(
                    l => l.Categorias.Any(
                        c => c.Id == filtro.Categoria.Value
                        )
                    );
            }

            if (!string.IsNullOrWhiteSpace(filtro.OrdenarPor))
            {
                switch (filtro.OrdenarPor.ToLower())
                {
                    case "titulo":
                        query = query.OrderBy(l => l.Titulo);
                        break;
                    case "anopublicacao":
                        query = query.OrderBy(l => l.AnoPublicacao);
                        break;
                    case "autor":
                        query = query.OrderBy(l => l.Autor.Nome);
                        break;
                    default:
                        break;
                }
            }

            return await query
                .Select(l => new LivroResponseDTO
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Autor = new AutorDTO
                    {
                        Id = l.Autor.Id,
                        Nome = l.Autor.Nome
                    },
                    AnoPublicacao = l.AnoPublicacao,
                    Categorias = l.Categorias
                        .Select(c => new CategoriaDTO {
                            Id = c.Id,
                            Nome = c.Nome
                        }).ToList()
                }).ToListAsync();
        }

        public async Task<LivroResponseDTO?> BuscarPorIdAsync(int id)
        {
            return await _context.Livros
                     .Where(l => l.Id == id)
                     .Select(l => new LivroResponseDTO
                     {
                         Id = l.Id,
                         Titulo = l.Titulo,
                         Autor = new AutorDTO
                         {
                             Id = l.Autor.Id,
                             Nome = l.Autor.Nome
                         },
                         AnoPublicacao = l.AnoPublicacao,
                         Categorias = l.Categorias
                            .Select(c => new CategoriaDTO
                            {
                                Id = c.Id,
                                Nome = c.Nome
                            }).ToList(),
                     }).FirstOrDefaultAsync();
        }
        public async Task<LivroResponseDTO> CadastrarAsync(LivroCreateDTO dto)
        {

            var autorIdExiste = await _context.Autores
                .AnyAsync(a => a.Id == dto.AutorId);

            if (!autorIdExiste)
            {
                throw new InvalidOperationException("Não existe autor com esse ID.");
            }

            /***
             * Verificar se já existe um livro cadastrado com as informações passadas
             * Trata-se de uma regra de negócio que deve ser validada no Service
             * A exceção lançada é tratada pelo controller
             */

            var existe = await _context.Livros
            .AnyAsync(l =>
                l.Titulo == dto.Titulo &&
                l.AutorId == dto.AutorId);

            if (existe)
            {
                throw new InvalidOperationException(
                    "Já existe um livro com este título e autor.");
            }

            var categoriasIds = dto.CategoriaIds?
                .Distinct()
                .ToList()
                ?? new List<int> ();

            var categorias = await _context.Categorias
                .Where(c => categoriasIds.Contains(c.Id))
                .ToListAsync();

            if (categorias.Count != categoriasIds.Count) 
            {
                throw new ArgumentException("Ao menos uma Categoria não existe.");
            }


            var livro = new Livro
            {
                Titulo = dto.Titulo,
                AutorId = dto.AutorId,
                AnoPublicacao = dto.AnoPublicacao,
                Categorias = categorias
            };

            _context.Livros.Add(livro);

            await _context.SaveChangesAsync();

            /*
            return new LivroResponseDTO
            {
                Id = livro.Id,
                Titulo = livro.Titulo,
                Autor = new AutorDTO
                {
                    Id = livro.Autor.Id,
                    Nome = livro.Autor.Nome
                },
                AnoPublicacao = livro.AnoPublicacao
            };
            */
            return await BuscarPorIdAsync(livro.Id) ?? throw new InvalidOperationException("Erro ao buscar o livro cadastrado.");

        }

        public async Task<LivroResponseDTO?> AlterarAsync(int id, LivroUpdateDTO dto)
        {
            //var livro = await _context.Livros.FindAsync(id);
            var livro = await _context.Livros
                .Include(l => l.Categorias)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
                return null;


            var autorIdExiste = await _context.Autores
                .AnyAsync(a => a.Id == dto.AutorId);

            if (!autorIdExiste)
            {
                throw new InvalidOperationException("Não existe autor com esse ID.");
            }

            /*
            var existe = await _context.Livros
                .AnyAsync(l =>
                    l.Titulo == dto.Titulo &&
                    l.AutorId == dto.AutorId);

            if (existe)
            {
                throw new InvalidOperationException(
                    "Já existe um livro com este título e autor.");
            }
            */

            var categoriasIds = dto.CategoriaIds?
                .Distinct()
                .ToList()
                ?? new List<int>();

            var categorias = await _context.Categorias
                .Where(c => categoriasIds.Contains(c.Id))
                .ToListAsync();

            if (categorias.Count != categoriasIds.Count)
            {
                throw new ArgumentException("Ao menos uma Categoria não existe.");
            }


            livro.Titulo = dto.Titulo;
            livro.AutorId = dto.AutorId;
            livro.AnoPublicacao = dto.AnoPublicacao; 
            livro.Categorias = categorias;

            await _context.SaveChangesAsync();

            return await BuscarPorIdAsync(livro.Id) ?? throw new InvalidOperationException("Erro ao buscar o livro cadastrado.");
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
                return false;

            _context.Livros.Remove(livro);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
