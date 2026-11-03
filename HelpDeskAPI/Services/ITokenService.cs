using HelpDeskAPI.Dtos;
using HelpDeskAPI.Models;

namespace HelpDeskAPI.Services;

public interface ITokenService
{
    TokenResponseDto GerarToken(Usuario usuario);
}
