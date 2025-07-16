using LocaCarros.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Account
{
    public interface IAuthenticate
    {
        Task<bool> Authenticate(string email, string password);
        Task<User> GetUserAsync(string email);
        Task<bool> RegisterUser(User user, string password);
        string GenerateToken(string email);
    }
}
