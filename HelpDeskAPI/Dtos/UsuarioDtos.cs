using System.ComponentModel.DataAnnotations;
using HelpDeskAPI.Models.Enums;

namespace HelpDeskAPI.Dtos;

public class UsuarioCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail em formato válido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O setor é obrigatório.")]
    [StringLength(80, ErrorMessage = "O setor deve ter no máximo 80 caracteres.")]
    public string Setor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O perfil é obrigatório.")]
    [EnumDataType(typeof(PerfilUsuario), ErrorMessage = "Perfil inválido.")]
    public PerfilUsuario Perfil { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}

public class UsuarioUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail em formato válido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O setor é obrigatório.")]
    [StringLength(80, ErrorMessage = "O setor deve ter no máximo 80 caracteres.")]
    public string Setor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O perfil é obrigatório.")]
    [EnumDataType(typeof(PerfilUsuario), ErrorMessage = "Perfil inválido.")]
    public PerfilUsuario Perfil { get; set; }

    public bool Ativo { get; set; }
}
