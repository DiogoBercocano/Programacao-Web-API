using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Data;

public class HelpDeskDbContext : DbContext
{
    public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<Interacao> Interacoes => Set<Interacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(120);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Setor).IsRequired().HasMaxLength(80);
        });

        modelBuilder.Entity<Chamado>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Descricao).IsRequired().HasMaxLength(2000);
            entity.Property(c => c.Categoria).IsRequired().HasMaxLength(60);

            entity.HasOne(c => c.Solicitante)
                .WithMany(u => u.ChamadosSolicitados)
                .HasForeignKey(c => c.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Tecnico)
                .WithMany(u => u.ChamadosAtendidos)
                .HasForeignKey(c => c.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Interacao>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Mensagem).IsRequired().HasMaxLength(1000);

            entity.HasOne(i => i.Chamado)
                .WithMany(c => c.Interacoes)
                .HasForeignKey(i => i.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Autor)
                .WithMany(u => u.Interacoes)
                .HasForeignKey(i => i.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nome = "Ana Souza", Email = "ana.souza@empresa.com", Setor = "Financeiro", Perfil = PerfilUsuario.Solicitante, Ativo = true },
            new Usuario { Id = 2, Nome = "Bruno Lima", Email = "bruno.lima@empresa.com", Setor = "Comercial", Perfil = PerfilUsuario.Solicitante, Ativo = true },
            new Usuario { Id = 3, Nome = "Carla Mendes", Email = "carla.mendes@empresa.com", Setor = "TI", Perfil = PerfilUsuario.Tecnico, Ativo = true },
            new Usuario { Id = 4, Nome = "Diego Ferreira", Email = "diego.ferreira@empresa.com", Setor = "TI", Perfil = PerfilUsuario.Tecnico, Ativo = true }
        );

        modelBuilder.Entity<Chamado>().HasData(
            new Chamado
            {
                Id = 1,
                Titulo = "Computador não liga",
                Descricao = "Ao pressionar o botão de energia, o computador não apresenta nenhuma resposta.",
                Categoria = "Hardware",
                Prioridade = PrioridadeChamado.Alta,
                Status = StatusChamado.Aberto,
                DataAbertura = new DateTime(2026, 9, 1, 9, 0, 0),
                DataFechamento = null,
                SolicitanteId = 1,
                TecnicoId = null
            },
            new Chamado
            {
                Id = 2,
                Titulo = "Erro ao acessar sistema de vendas",
                Descricao = "Sistema apresenta erro 500 ao tentar gerar um novo pedido.",
                Categoria = "Software",
                Prioridade = PrioridadeChamado.Critica,
                Status = StatusChamado.EmAtendimento,
                DataAbertura = new DateTime(2026, 9, 3, 14, 30, 0),
                DataFechamento = null,
                SolicitanteId = 2,
                TecnicoId = 3
            },
            new Chamado
            {
                Id = 3,
                Titulo = "Solicitação de acesso à rede Wi-Fi",
                Descricao = "Notebook novo não consegue se conectar à rede corporativa.",
                Categoria = "Rede",
                Prioridade = PrioridadeChamado.Media,
                Status = StatusChamado.Fechado,
                DataAbertura = new DateTime(2026, 8, 20, 10, 15, 0),
                DataFechamento = new DateTime(2026, 8, 20, 11, 0, 0),
                SolicitanteId = 1,
                TecnicoId = 4
            }
        );

        modelBuilder.Entity<Interacao>().HasData(
            new Interacao { Id = 1, Mensagem = "Chamado registrado pelo solicitante.", DataRegistro = new DateTime(2026, 9, 1, 9, 0, 0), Interna = false, ChamadoId = 1, AutorId = 1 },
            new Interacao { Id = 2, Mensagem = "Chamado assumido para atendimento.", DataRegistro = new DateTime(2026, 9, 3, 14, 45, 0), Interna = false, ChamadoId = 2, AutorId = 3 },
            new Interacao { Id = 3, Mensagem = "Verificado log do servidor, aguardando confirmação do time de infraestrutura.", DataRegistro = new DateTime(2026, 9, 3, 15, 30, 0), Interna = true, ChamadoId = 2, AutorId = 3 },
            new Interacao { Id = 4, Mensagem = "Configuração de rede realizada no notebook.", DataRegistro = new DateTime(2026, 8, 20, 10, 50, 0), Interna = false, ChamadoId = 3, AutorId = 4 },
            new Interacao { Id = 5, Mensagem = "Problema resolvido, chamado encerrado.", DataRegistro = new DateTime(2026, 8, 20, 11, 0, 0), Interna = false, ChamadoId = 3, AutorId = 1 }
        );
    }
}
