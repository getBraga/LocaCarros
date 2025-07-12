using LocaCarros.Domain.Entities.Identity;
using LocaCarros.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Tests
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldCreateUser_WhenDataIsValid()
        {
            var user = new User("email@teste.com", "usuario", "Nome", "Sobrenome", "11999999999");
            Assert.Equal("email@teste.com", user.Email);
            Assert.Equal("usuario", user.Username);
            Assert.Equal("Nome", user.FirstName);
            Assert.Equal("Sobrenome", user.LastName);
            Assert.Equal("11999999999", user.PhoneNumber);
            Assert.NotNull(user.Id);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenEmailIsInvalid()
        {
            Assert.Throws<DomainException>(() =>
                new User("email_invalido", "usuario", "Nome", "Sobrenome", "11999999999"));
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenPhoneNumberIsInvalid()
        {
            Assert.Throws<DomainException>(() =>
                new User("email@teste.com", "usuario", "Nome", "Sobrenome", "123"));
        }

        [Fact]
        public void Constructor_ShouldThrowDomainException_WhenRequiredFieldsAreEmpty()
        {
            Assert.Throws<DomainException>(() =>
                new User("", "usuario", "Nome", "Sobrenome", "11999999999"));
            Assert.Throws<DomainException>(() =>
                new User("email@teste.com", "", "Nome", "Sobrenome", "11999999999"));
            Assert.Throws<DomainException>(() =>
                new User("email@teste.com", "usuario", "", "Sobrenome", "11999999999"));
            Assert.Throws<DomainException>(() =>
                new User("email@teste.com", "usuario", "Nome", "", "11999999999"));
        }

        [Fact]
        public void UpdateUserProfile_ShouldUpdateFields_WhenDataIsValid()
        {
            var user = new User("email@teste.com", "usuario", "Nome", "Sobrenome", "11999999999");
            user.UpdateUserProfile("novo@teste.com", "novousuario", "NovoNome", "NovoSobrenome", "11988888888");

            Assert.Equal("novo@teste.com", user.Email);
            Assert.Equal("novousuario", user.Username);
            Assert.Equal("NovoNome", user.FirstName);
            Assert.Equal("NovoSobrenome", user.LastName);
            Assert.Equal("11988888888", user.PhoneNumber);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public void UpdateUserProfile_ShouldThrowDomainException_WhenEmailIsInvalid()
        {
            var user = new User("email@teste.com", "usuario", "Nome", "Sobrenome", "11999999999");
            Assert.Throws<DomainException>(() =>
                user.UpdateUserProfile("email_invalido", "usuario", "Nome", "Sobrenome", "11999999999"));
        }
    }
}
