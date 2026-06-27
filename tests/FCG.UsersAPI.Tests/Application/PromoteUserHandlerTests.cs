using FCG.UsersAPI.Application.Users.Commands.PromoteUser;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Enums;
using FCG.UsersAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.UsersAPI.Tests.Application;

public class PromoteUserHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly PromoteUserCommandHandler _handler;

    public PromoteUserHandlerTests()
        => _handler = new PromoteUserCommandHandler(_userRepo);

    [Fact]
    public async Task Handle_ShouldPromoteUser_WhenUserExists()
    {
        var user = User.Create("Alice", "alice@email.com", "Senha@123");
        _userRepo.GetByIdAsync(user.Id).Returns(user);

        var result = await _handler.Handle(
            new PromoteUserCommand(user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserRole.Admin, user.Role);
        await _userRepo.Received(1).UpdateAsync(user);
        await _userRepo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        _userRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((User?)null);

        var result = await _handler.Handle(
            new PromoteUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrado", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyAdmin()
    {
        var user = User.Create("Alice", "alice@email.com", "Senha@123");
        user.PromoteToAdmin();
        _userRepo.GetByIdAsync(user.Id).Returns(user);

        var result = await _handler.Handle(
            new PromoteUserCommand(user.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("já é administrador", result.Error);
    }
}
