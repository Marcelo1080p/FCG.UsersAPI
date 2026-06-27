using FCG.UsersAPI.Application.Users.Queries.GetAllUsers;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.UsersAPI.Tests.Application;

public class GetAllUsersHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly GetAllUsersHandler _handler;

    public GetAllUsersHandlerTests()
        => _handler = new GetAllUsersHandler(_userRepo);

    [Fact]
    public async Task Handle_ShouldReturnAllUsers()
    {
        var users = new List<User>
        {
            User.Create("Alice", "alice@email.com", "Senha@123"),
            User.Create("Bob", "bob@email.com", "Senha@456")
        };
        _userRepo.GetAllAsync().Returns(users);

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        _userRepo.GetAllAsync().Returns(new List<User>());

        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
