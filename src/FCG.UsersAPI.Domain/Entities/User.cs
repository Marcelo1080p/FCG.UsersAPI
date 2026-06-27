using System.Text.RegularExpressions;
using FCG.UsersAPI.Domain.Enums;
using FCG.UsersAPI.Domain.Exceptions;

namespace FCG.UsersAPI.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string plainPassword)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidatePassword(plainPassword);

        return new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool VerifyPassword(string plainPassword)
        => BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);

    public void Deactivate() => IsActive = false;

    public void PromoteToAdmin() => Role = UserRole.Admin;

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        if (!regex.IsMatch(email))
            throw new DomainException("Formato de e-mail inválido.");
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length < 8)
            throw new DomainException("Senha deve ter no mínimo 8 caracteres.");

        if (!password.Any(char.IsLetter))
            throw new DomainException("Senha deve conter letras.");

        if (!password.Any(char.IsDigit))
            throw new DomainException("Senha deve conter números.");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new DomainException("Senha deve conter caracteres especiais.");
    }
}
