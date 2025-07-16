using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.Authenticate;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.API.Tests
{
    public class UserControllerTest
    {
        private readonly Mock<IAuthenticateServiceApplication> _userServiceMock;
        private readonly UserController _userController;
        public UserControllerTest()
        {
            _userServiceMock = new Mock<IAuthenticateServiceApplication>();
            _userController = new UserController(_userServiceMock.Object);
        }
        private static UserTokenDTO GetUserTokenDtoMock() => new()
        {
            Email = "teste@locacarros.com",
            Username = "testeUser",
            Password = "SenhaForte123!",

        };
        private void SimularUsuarioAutenticado(string email)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        [Fact]
        public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
        {


            _userServiceMock.Setup(s => s.GetUser(GetUserTokenDtoMock().Email)).ReturnsAsync(GetUserTokenDtoMock());
            SimularUsuarioAutenticado(GetUserTokenDtoMock().Email);
            var resultado = await _userController.GetUser();
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var usuarioRetornado = Assert.IsType<UserTokenDTO>(okResult.Value);
            Assert.Equal(GetUserTokenDtoMock().Email, usuarioRetornado.Email);

        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
        {
            var loginDto = new LoginDTO
            {
                Email = "locacarros@email.com",
                Password = "S&nh@Forte1"
            };
            _userServiceMock.Setup(s => s.AuthenticateAsync(loginDto)).ReturnsAsync(false);
            var userResult = await _userController.Login(loginDto);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(userResult.Result);
            var message = unauthorizedResult.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value);
            Assert.Equal("Authentication failed.", message);
        }
        [Fact]
        public async Task Login_DeveRetornarOk_QuandoCredenciaisValidas()
        {
            var loginDto = new LoginDTO
            {
                Email = "locacarros@email.com",
                Password = "S&nh@Forte1"
            };
            var tokenEsperado = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            _userServiceMock.Setup(s => s.AuthenticateAsync(loginDto)).ReturnsAsync(true);
            _userServiceMock.Setup(s => s.GenerateToken(loginDto.Email)).Returns(tokenEsperado);
            var userResult = await _userController.Login(loginDto);
            var okResult = Assert.IsType<OkObjectResult>(userResult.Result);
            var tokenProperty = okResult.Value?.GetType().GetProperty("data");
            Assert.NotNull(tokenProperty);

            var tokenRetornado = tokenProperty.GetValue(okResult.Value) as string;
            Assert.False(string.IsNullOrWhiteSpace(tokenRetornado));

            Assert.StartsWith("ey", tokenRetornado);
        }
        [Fact]
        public async Task Register_DeveRetornarOk_QuandoRegistroForBemSucedido()
        {
            var registerDto = new RegisterDTO
            {
                Email = "locacarros@email.com",
                Password = "S&nh@Forte1",
                ConfirmPassword = "S&nh@Forte1",
                FirstName = "Loca",
                LastName = "Carros",
                Username = "locacarrosUser"
            };

            _userServiceMock.Setup(s => s.RegisterUserAsync(registerDto)).ReturnsAsync(true);
            var result = await _userController.Register(registerDto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var message = okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Usuário cadastrado com sucesso!", message);
        }
        [Fact]
        public async Task Register_DeveRetornarBadRequest_QuandoRegistroFalhar()
        {

            var registerDto = new RegisterDTO
            {
                Email = "locacarros@email.com",
                Password = "S&nh@Forte1",
                ConfirmPassword = "S&nh@Forte1",
                FirstName = "Loca",
                LastName = "Carros",
                Username = "locacarrosUser"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerDto)).ReturnsAsync(false);
            var result = await _userController.Register(registerDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("Erro ao cadastrar usuário!", message);
        }
    }
}
