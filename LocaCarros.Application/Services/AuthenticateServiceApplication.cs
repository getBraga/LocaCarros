using AutoMapper;
using LocaCarros.Application.DTOs.Authenticate;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Account;
using LocaCarros.Domain.Entities.Identity;
namespace LocaCarros.Application.Services
{
    public class AuthenticateServiceApplication : IAuthenticateServiceApplication
    {
        private readonly IAuthenticate _authenticateServiceInfra;
        private readonly IMapper _mapper;
        public AuthenticateServiceApplication(IAuthenticate authenticateService, IMapper mapper)
        {
            _authenticateServiceInfra = authenticateService ?? throw new ArgumentNullException(nameof(authenticateService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<bool> AuthenticateAsync(LoginDTO loginDTO)
        {
            var authenticateTask = await _authenticateServiceInfra.Authenticate(loginDTO.Email, loginDTO.Password);
            return authenticateTask;
        }

        public async Task<UserTokenDTO> GetUser(string email)
        {

            var user = await _authenticateServiceInfra.GetUserAsync(email);
            return _mapper.Map<UserTokenDTO>(user);
        }

        public string GenerateToken(string email)
        {
          var token = _authenticateServiceInfra.GenerateToken(email);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Token generation failed.");
            }
            return token;
        }

        public async Task<bool> RegisterUserAsync(RegisterDTO registerDTO)
        {
            var registerMapper = _mapper.Map<User>(registerDTO);
            var registerTask = await _authenticateServiceInfra.RegisterUser(registerMapper, registerDTO.Password);
            return registerTask;
        }
    }
}
