using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpDeskAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Setor = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chamados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chamados_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamados_Usuarios_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Interacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Interna = table.Column<bool>(type: "bit", nullable: false),
                    ChamadoId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interacoes_Chamados_ChamadoId",
                        column: x => x.ChamadoId,
                        principalTable: "Chamados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interacoes_Usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "Email", "Nome", "Perfil", "Setor" },
                values: new object[,]
                {
                    { 1, true, "ana.souza@empresa.com", "Ana Souza", 1, "Financeiro" },
                    { 2, true, "bruno.lima@empresa.com", "Bruno Lima", 1, "Comercial" },
                    { 3, true, "carla.mendes@empresa.com", "Carla Mendes", 2, "TI" },
                    { 4, true, "diego.ferreira@empresa.com", "Diego Ferreira", 2, "TI" }
                });

            migrationBuilder.InsertData(
                table: "Chamados",
                columns: new[] { "Id", "Categoria", "DataAbertura", "DataFechamento", "Descricao", "Prioridade", "SolicitanteId", "Status", "TecnicoId", "Titulo" },
                values: new object[,]
                {
                    { 1, "Hardware", new DateTime(2026, 9, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), null, "Ao pressionar o botão de energia, o computador não apresenta nenhuma resposta.", 3, 1, 1, null, "Computador não liga" },
                    { 2, "Software", new DateTime(2026, 9, 3, 14, 30, 0, 0, DateTimeKind.Unspecified), null, "Sistema apresenta erro 500 ao tentar gerar um novo pedido.", 4, 2, 2, 3, "Erro ao acessar sistema de vendas" },
                    { 3, "Rede", new DateTime(2026, 8, 20, 10, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), "Notebook novo não consegue se conectar à rede corporativa.", 2, 1, 4, 4, "Solicitação de acesso à rede Wi-Fi" }
                });

            migrationBuilder.InsertData(
                table: "Interacoes",
                columns: new[] { "Id", "AutorId", "ChamadoId", "DataRegistro", "Interna", "Mensagem" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2026, 9, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), false, "Chamado registrado pelo solicitante." },
                    { 2, 3, 2, new DateTime(2026, 9, 3, 14, 45, 0, 0, DateTimeKind.Unspecified), false, "Chamado assumido para atendimento." },
                    { 3, 3, 2, new DateTime(2026, 9, 3, 15, 30, 0, 0, DateTimeKind.Unspecified), true, "Verificado log do servidor, aguardando confirmação do time de infraestrutura." },
                    { 4, 4, 3, new DateTime(2026, 8, 20, 10, 50, 0, 0, DateTimeKind.Unspecified), false, "Configuração de rede realizada no notebook." },
                    { 5, 1, 3, new DateTime(2026, 8, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), false, "Problema resolvido, chamado encerrado." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_SolicitanteId",
                table: "Chamados",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_TecnicoId",
                table: "Chamados",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Interacoes_AutorId",
                table: "Interacoes",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Interacoes_ChamadoId",
                table: "Interacoes",
                column: "ChamadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interacoes");

            migrationBuilder.DropTable(
                name: "Chamados");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
