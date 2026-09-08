using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;

namespace HelpDeskAPI.Data;

public static class RepositorioEmMemoria
{
    public static List<Usuario> Usuarios { get; } = new();
    public static List<Chamado> Chamados { get; } = new();
    public static List<Interacao> Interacoes { get; } = new();

    static RepositorioEmMemoria()
    {
        var ana = new Usuario { Id = 1, Nome = "Ana Souza", Email = "ana.souza@empresa.com", Setor = "Financeiro", Perfil = PerfilUsuario.Solicitante, Ativo = true };
        var bruno = new Usuario { Id = 2, Nome = "Bruno Lima", Email = "bruno.lima@empresa.com", Setor = "Comercial", Perfil = PerfilUsuario.Solicitante, Ativo = true };
        var carla = new Usuario { Id = 3, Nome = "Carla Mendes", Email = "carla.mendes@empresa.com", Setor = "TI", Perfil = PerfilUsuario.Tecnico, Ativo = true };
        var diego = new Usuario { Id = 4, Nome = "Diego Ferreira", Email = "diego.ferreira@empresa.com", Setor = "TI", Perfil = PerfilUsuario.Tecnico, Ativo = true };

        Usuarios.AddRange(new[] { ana, bruno, carla, diego });

        var chamado1 = new Chamado
        {
            Id = 1,
            Titulo = "Computador não liga",
            Descricao = "Ao pressionar o botão de energia, o computador não apresenta nenhuma resposta.",
            Categoria = "Hardware",
            Prioridade = PrioridadeChamado.Alta,
            Status = StatusChamado.Aberto,
            DataAbertura = new DateTime(2026, 9, 1, 9, 0, 0),
            DataFechamento = null,
            SolicitanteId = ana.Id,
            TecnicoId = null
        };

        var chamado2 = new Chamado
        {
            Id = 2,
            Titulo = "Erro ao acessar sistema de vendas",
            Descricao = "Sistema apresenta erro 500 ao tentar gerar um novo pedido.",
            Categoria = "Software",
            Prioridade = PrioridadeChamado.Critica,
            Status = StatusChamado.EmAtendimento,
            DataAbertura = new DateTime(2026, 9, 3, 14, 30, 0),
            DataFechamento = null,
            SolicitanteId = bruno.Id,
            TecnicoId = carla.Id
        };

        var chamado3 = new Chamado
        {
            Id = 3,
            Titulo = "Solicitação de acesso à rede Wi-Fi",
            Descricao = "Notebook novo não consegue se conectar à rede corporativa.",
            Categoria = "Rede",
            Prioridade = PrioridadeChamado.Media,
            Status = StatusChamado.Fechado,
            DataAbertura = new DateTime(2026, 8, 20, 10, 15, 0),
            DataFechamento = new DateTime(2026, 8, 20, 11, 0, 0),
            SolicitanteId = ana.Id,
            TecnicoId = diego.Id
        };

        Chamados.AddRange(new[] { chamado1, chamado2, chamado3 });

        var interacao1 = new Interacao { Id = 1, Mensagem = "Chamado registrado pelo solicitante.", DataRegistro = new DateTime(2026, 9, 1, 9, 0, 0), Interna = false, ChamadoId = chamado1.Id, AutorId = ana.Id };
        var interacao2 = new Interacao { Id = 2, Mensagem = "Chamado assumido para atendimento.", DataRegistro = new DateTime(2026, 9, 3, 14, 45, 0), Interna = false, ChamadoId = chamado2.Id, AutorId = carla.Id };
        var interacao3 = new Interacao { Id = 3, Mensagem = "Verificado log do servidor, aguardando confirmação do time de infraestrutura.", DataRegistro = new DateTime(2026, 9, 3, 15, 30, 0), Interna = true, ChamadoId = chamado2.Id, AutorId = carla.Id };
        var interacao4 = new Interacao { Id = 4, Mensagem = "Configuração de rede realizada no notebook.", DataRegistro = new DateTime(2026, 8, 20, 10, 50, 0), Interna = false, ChamadoId = chamado3.Id, AutorId = diego.Id };
        var interacao5 = new Interacao { Id = 5, Mensagem = "Problema resolvido, chamado encerrado.", DataRegistro = new DateTime(2026, 8, 20, 11, 0, 0), Interna = false, ChamadoId = chamado3.Id, AutorId = ana.Id };

        Interacoes.AddRange(new[] { interacao1, interacao2, interacao3, interacao4, interacao5 });

        foreach (var chamado in Chamados)
        {
            chamado.Solicitante = Usuarios.First(u => u.Id == chamado.SolicitanteId);
            chamado.Tecnico = chamado.TecnicoId.HasValue
                ? Usuarios.First(u => u.Id == chamado.TecnicoId.Value)
                : null;
            chamado.Interacoes = Interacoes.Where(i => i.ChamadoId == chamado.Id).ToList();
        }

        foreach (var interacao in Interacoes)
        {
            interacao.Chamado = Chamados.First(c => c.Id == interacao.ChamadoId);
            interacao.Autor = Usuarios.First(u => u.Id == interacao.AutorId);
        }
    }
}
