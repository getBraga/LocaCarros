using LocaCarros.Domain.Account;
using LocaCarros.Domain.Entities.Identity;
using LocaCarros.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Infra.Data.Identity
{
    public class AuthenticateService : IAuthenticate
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthenticateService(UserManager<ApplicationUser> userMaganer,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userMaganer ?? throw new ArgumentNullException(nameof(userMaganer));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<User> GetUserAsync(string email) {

            var user = await _userManager.FindByEmailAsync(email);
         
            if (user == null)
                throw new DomainException("O Usuário não foi encontrado!");
            var userMap = new User(user.Email!, user.UserName!, user.FirstName, user.LastName, user.PhoneNumber!);
           
            return userMap;
        }
        public async Task<bool> Authenticate(string email, string password)
        {
       
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
       
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
                return false;

            //await _signInManager.SignInAsync(user, isPersistent: false);
        
            return true;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }


        public async Task<bool> RegisterUser(User user, string password)
        {
            ApplicationUser appplicationUser = new()
            {
                UserName = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName


            };
            var verificaUser = await _userManager.FindByEmailAsync(user.Email);
            if (verificaUser != null)
                throw new DomainException($"O email {user.Email} já está sendo utilizado");
            var result = await _signInManager.UserManager.CreateAsync(appplicationUser, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(appplicationUser, isPersistent: false);
                return true;
            }
            return result.Succeeded;
        }

        public string GenerateToken(string email)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = new[]
          {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: expiration,

                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
