using LocaCarros.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Entities.Identity
{
    public class User
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string Email { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        [ExcludeFromCodeCoverage]
        private User()
        {
            
        }
        public User(string email, string username, string firstName, string lastName, string phoneNumber)
        {
            SetUsername(username);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetFirstName(firstName);
            SetLastName(lastName);
        }
      
     
        private void SetUsername(string username)
        {
            ValidateRequiredField(username, "Nome de usuário");
            Username = username;
        }
        private void SetFirstName(string firstName)
        {
            ValidateRequiredField(firstName, "Primeiro nome de usuário");
            FirstName = firstName;
        }
        private void SetLastName(string lastName)
        {
            ValidateRequiredField(lastName, "Ultimo nome de usuário");
            LastName = lastName;
        }
        private void SetEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new DomainException("O Campo de email não pode ser vazio!");
            if (!IsValidEmail(email))
                throw new DomainException("O e-mail informado é inválido.");
            Email = email;

        }

     

        private bool IsValidEmail(string email)
        {
            var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailRegex);
        }
        private void SetPhoneNumber(string phoneNumber)
        {
       
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrEmpty(phoneNumber) && digitsOnly.Length < 10 || digitsOnly.Length > 11)
                throw new DomainException("Número de telefone inválido. Deve conter 10 ou 11 dígitos.");

            PhoneNumber = digitsOnly;
        }
        public void UpdateUserProfile(string email, string username, string firstName, string lastName, string phoneNumber)
        {
            SetUsername(username);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetFirstName(firstName);
            SetLastName(lastName);
            UpdatedAt = DateTime.UtcNow;
        }
        private void ValidateRequiredField(string field, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new DomainException($"{fieldName} é obrigatório");
        }
    }
}
