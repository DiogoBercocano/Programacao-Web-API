using HelpDeskAPI.Models;
using HelpDeskAPI.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Data;

public static class DbSeeder
{
    public const string SenhaPadrao = "Senha@123";
    public const string SenhaAdmin = "Admin@123";

    public static async Task SeedAsync(HelpDeskDbContext context, IPasswordHasher<Usuario> hasher)
    {
        if (!await context.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador))
        {
            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@empresa.com",
                Setor = "TI",
                Perfil = PerfilUsuario.Administrador,
                Ativo = true
            };
            admin.SenhaHash = hasher.HashPassword(admin, SenhaAdmin);
            context.Usuarios.Add(admin);
        }

        // Usuários criados antes da autenticação não possuem senha.
        var semSenha = await context.Usuarios.Where(u => u.SenhaHash == "").ToListAsync();
        foreach (var usuario in semSenha)
        {
            usuario.SenhaHash = hasher.HashPassword(usuario, SenhaPadrao);
        }

        await context.SaveChangesAsync();
    }
}
