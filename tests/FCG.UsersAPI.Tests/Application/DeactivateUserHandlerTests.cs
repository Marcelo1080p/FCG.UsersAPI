using FCG.UsersAPI.Application.Users.Commands.DeactivateUser;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.UsersAPI.Tests.Application;

public class DeactivateUserHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserHandlerTests()
        => _handler = new DeactivateUserCommandHandler(_userRepo);

    [Fact]
    public async Task Handle_ShouldDeactivateUser_WhenUserExists()
    {
        var user = User.Create("Alice", "alice@email.com", "Senha@123");
        _userRepo.GetByIdAsync(user.Id).Returns(user);

        var result = await _handler.Handle(
            new DeactivateUserCommand(user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(user.IsActive);
        await _userRepo.Received(1).UpdateAsync(user);
        await _userRepo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        _userRepo.GetByIdAsync(Arg.Any<Guid>()).Returns((User?)null);

        var result = await _handler.Handle(
            new DeactivateUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("não encontrado", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyInactive()
    {
        var user = User.Create("Alice", "alice@email.com", "Senha@123");
        user.Deactivate();
        _userRepo.GetByIdAsync(user.Id).Returns(user);

        var result = await _handler.Handle(
            new DeactivateUserCommand(user.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("já está inativo", result.Error);
    }
}
