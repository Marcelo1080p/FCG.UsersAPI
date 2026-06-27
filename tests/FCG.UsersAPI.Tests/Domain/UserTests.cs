using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Enums;
using FCG.UsersAPI.Domain.Exceptions;

namespace FCG.UsersAPI.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("Marcelo", "marcelo@email.com", "Senha@123");

        Assert.Equal("Marcelo", user.Name);
        Assert.Equal("marcelo@email.com", user.Email);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    public void Create_ShouldThrow_WhenEmailIsInvalid(string email)
    {
        Assert.Throws<DomainException>(() =>
            User.Create("Marcelo", email, "Senha@123"));
    }

    [Theory]
    [InlineData("short1@")]
    [InlineData("senhasemnum")]
    [InlineData("SEMSPECIAL1")]
    [InlineData("12345678@")]
    public void Create_ShouldThrow_WhenPasswordIsWeak(string password)
    {
        Assert.Throws<DomainException>(() =>
            User.Create("Marcelo", "marcelo@email.com", password));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = User.Create("Marcelo", "marcelo@email.com", "Senha@123");
        user.Deactivate();
        Assert.False(user.IsActive);
    }

    [Fact]
    public void PromoteToAdmin_ShouldChangeRole()
    {
        var user = User.Create("Marcelo", "marcelo@email.com", "Senha@123");
        user.PromoteToAdmin();
        Assert.Equal(UserRole.Admin, user.Role);
    }
}
