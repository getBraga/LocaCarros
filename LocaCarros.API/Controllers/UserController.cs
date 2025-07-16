using LocaCarros.Application.DTOs.Authenticate;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LocaCarros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticateServiceApplication _authenticateServiceApplication;
        public UserController(IAuthenticateServiceApplication authenticateServiceApplication)
        {
            _authenticateServiceApplication = authenticateServiceApplication ?? throw new ArgumentNullException(nameof(authenticateServiceApplication));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserTokenDTO>> GetUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _authenticateServiceApplication.GetUser(email!);
            return Ok(result);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserTokenDTO>> Login(LoginDTO loginDTO)
        {
           
            var result = await _authenticateServiceApplication.AuthenticateAsync(loginDTO);
            if (result)
            {
                var token = _authenticateServiceApplication.GenerateToken(loginDTO.Email);
                return Ok(new {success = true, data = token });
            }
            return Unauthorized(new { success = false, message = "Authentication failed." });
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<bool>> Register(RegisterDTO registerDTO)
        {
            var result = await _authenticateServiceApplication.RegisterUserAsync(registerDTO);
            if (result)
            {
                return Ok(new {success = true, message= "Usuário cadastrado com sucesso!" });
            }
            return BadRequest(new {success= false, message="Erro ao cadastrar usuário!"});
        }
       
    }
}
