

using AutoMapper;
using LocaCarros.Application.DTOs.Authenticate;
using LocaCarros.Application.Interfaces;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Account;
using LocaCarros.Domain.Entities.Identity;
using Moq;

namespace LocaCarros.Application.Tests
{
    public class AuthenticateServiceApplicationTest
    {
        private readonly Mock<IAuthenticate> _authenticateServiceInfra;
        private readonly Mock<IMapper> _mapper;
        private readonly AuthenticateServiceApplication _authenticateServiceApplication;
        private readonly User _user;
        private readonly UserTokenDTO _userTokenDTO;
        private readonly RegisterDTO _registerDTO;
        public AuthenticateServiceApplicationTest()
        {
            _authenticateServiceInfra = new Mock<IAuthenticate>();
            _mapper = new Mock<IMapper>();
            _authenticateServiceApplication = new AuthenticateServiceApplication(_authenticateServiceInfra.Object, _mapper.Object);
            _user = new User("email@email.com", "teste", "Teste", "Teste Name", "98614245512");

            _userTokenDTO = new UserTokenDTO
            {
                Email = _user.Email,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Id = _user.Id,
                Username = _user.Username,
                PhoneNumber = _user.PhoneNumber
            };
            _registerDTO = new RegisterDTO
            {
                Username = "teste",
                Email = "email@email.com",
                Password = "12234",
                ConfirmPassword = "12234",
                FirstName = "Teste",
                LastName  = "Teste Name"
            };
            }

        [Fact]
        public async Task AuthenticateAsync_DeveRetornarTrueQuandooEmailESenhaValidos()
        {
            var loginDTO = new LoginDTO
            {
                Email = "teste@teste.com",
                Password = "Teste@123"
            };
            
            _authenticateServiceInfra.Setup(a => a.Authenticate(loginDTO.Email, loginDTO.Password)).ReturnsAsync(true);
            
            var resultTrue = await _authenticateServiceApplication.AuthenticateAsync(loginDTO);
            
            Assert.True(resultTrue);
        }
        [Fact]
        public async Task AuthenticateAsync_DeveRetornarFalseQuandoEmailInvalido()
        {
            var loginDTO = new LoginDTO
            {
                Email = "teste@teste.com",
                Password = "Teste@123"
            };
            
            _authenticateServiceInfra.Setup(a => a.Authenticate(loginDTO.Email, loginDTO.Password)).ReturnsAsync(false);
            
            var resultFalse = await _authenticateServiceApplication.AuthenticateAsync(loginDTO);
            
            Assert.False(resultFalse);

        }
        [Fact]
        public async Task AuthenticateAsync_DeveRetornarFalseQuandoPasswordInvalido()
        {
            var loginDTO = new LoginDTO
            {
                Email = "teste@teste.com",
                Password = "Teste@123"
            };

            _authenticateServiceInfra.Setup(a => a.Authenticate(loginDTO.Email, loginDTO.Password)).ReturnsAsync(false);
       
            var resultFalse = await _authenticateServiceApplication.AuthenticateAsync(loginDTO);
           
            Assert.False(resultFalse);
        }

        [Fact]
        public async Task GetUser_DeveRetonarUsuario_QuandoEmailForValido()
        {

            _authenticateServiceInfra.Setup(a => a.GetUserAsync(_user.Email)).ReturnsAsync(_user);   
            _mapper.Setup(m => m.Map<UserTokenDTO>(_user)).Returns(_userTokenDTO);
            
            var resultEmail = await _authenticateServiceApplication.GetUser(_user.Email);
            
            Assert.NotNull(resultEmail);   
            Assert.Equal(_user.Email, resultEmail.Email);
        }

        [Fact]
        public void GetUser_DeveRetornarNulo_QuandoTokenNaoForGerado() {
            var email = "email@valido.com";
             _authenticateServiceInfra.Setup(g => g.GenerateToken(email)).Returns(string.Empty);
            var result = Assert.Throws<InvalidOperationException>(() => _authenticateServiceApplication.GenerateToken(email));
            Assert.Equal("Token generation failed.", result.Message);

        }

        [Fact]
        public async Task Register_DeveRetornarUsuario_QuadoRegistrarOk()
        {
            _mapper.Setup(m => m.Map<User>(_registerDTO)).Returns(_user);
            _authenticateServiceInfra.Setup(r => r.RegisterUser(_user, _registerDTO.Password)).ReturnsAsync(true);
            var registerResult = await _authenticateServiceApplication.RegisterUserAsync(_registerDTO);
            Assert.True(registerResult);
        }
    }
}
