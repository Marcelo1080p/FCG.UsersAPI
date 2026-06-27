using FCG.UsersAPI.Application.Users.Commands.Login;
using FCG.UsersAPI.Application.Interfaces;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.UsersAPI.Tests.Application;

public class LoginHandlerTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
        => _handler = new LoginHandler(_repo, _tokenService);

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = User.Create("Marcelo", "marcelo@email.com", "Senha@123");
        _repo.GetByEmailAsync("marcelo@email.com").Returns(user);
        _tokenService.Generate(user).Returns("jwt-token");

        var result = await _handler.Handle(
            new LoginCommand("marcelo@email.com", "Senha@123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        _repo.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);

        var result = await _handler.Handle(
            new LoginCommand("x@x.com", "Senha@123"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPasswordIsWrong()
    {
        var user = User.Create("Marcelo", "marcelo@email.com", "Senha@123");
        _repo.GetByEmailAsync("marcelo@email.com").Returns(user);

        var result = await _handler.Handle(
            new LoginCommand("marcelo@email.com", "SenhaErrada@1"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
