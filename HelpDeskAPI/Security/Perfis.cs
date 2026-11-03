namespace HelpDeskAPI.Security;

public static class Perfis
{
    public const string Solicitante = "Solicitante";
    public const string Tecnico = "Tecnico";
    public const string Administrador = "Administrador";

    public const string TecnicoOuAdministrador = Tecnico + "," + Administrador;
}
