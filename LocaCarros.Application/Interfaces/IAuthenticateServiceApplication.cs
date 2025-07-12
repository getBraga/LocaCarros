using LocaCarros.Application.DTOs.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Interfaces
{
    public interface IAuthenticateServiceApplication
    {
        Task<bool> AuthenticateAsync(LoginDTO loginDto);
        Task<bool> RegisterUserAsync(RegisterDTO registerDTO);
        Task<UserTokenDTO> GetUser(string email);
        string GenerateToken(string email);
        Task LogoutAsync();
    }
}
