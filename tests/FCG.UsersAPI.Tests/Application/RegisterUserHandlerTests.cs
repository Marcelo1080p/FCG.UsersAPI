using FCG.UsersAPI.Application.Users.Commands.RegisterUser;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Interfaces;
using MassTransit;
using NSubstitute;

namespace FCG.UsersAPI.Tests.Application;

public class RegisterUserHandlerTests
{
    private readonly IUserRepository _repo = Substitute.For<IUserRepository>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
        => _handler = new RegisterUserHandler(_repo, _publishEndpoint);

    [Fact]
    public async Task Handle_ShouldAddUser_WhenEmailIsNew()
    {
        _repo.ExistsAsync("novo@email.com").Returns(false);

        var cmd = new RegisterUserCommand("Marcelo", "novo@email.com", "Senha@123");
        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _repo.Received(1).AddAsync(Arg.Any<User>());
        await _repo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenEmailAlreadyExists()
    {
        _repo.ExistsAsync("existente@email.com").Returns(true);

        var cmd = new RegisterUserCommand("Outro", "existente@email.com", "Senha@123");
        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("já cadastrado", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ShouldPublishEvent_WhenUserIsCreated()
    {
        _repo.ExistsAsync("novo@email.com").Returns(false);

        var cmd = new RegisterUserCommand("Marcelo", "novo@email.com", "Senha@123");
        await _handler.Handle(cmd, CancellationToken.None);

        await _publishEndpoint.Received(1).Publish(
            Arg.Any<FCG.UsersAPI.Application.Events.UserCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }
}
