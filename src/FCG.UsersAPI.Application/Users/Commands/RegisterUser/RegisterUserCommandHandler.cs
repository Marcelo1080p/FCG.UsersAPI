using FCG.UsersAPI.Application.Common;
using FCG.UsersAPI.Application.Events;
using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Exceptions;
using FCG.UsersAPI.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterUserHandler(IUserRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await _repo.ExistsAsync(cmd.Email, ct))
            return Result<Guid>.Fail("E-mail já cadastrado.");

        try
        {
            var user = User.Create(cmd.Name, cmd.Email, cmd.Password);
            await _repo.AddAsync(user, ct);
            await _repo.SaveChangesAsync(ct);

            await _publishEndpoint.Publish(
                new UserCreatedEvent(user.Id, user.Name, user.Email), ct);

            return Result<Guid>.Ok(user.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Fail(ex.Message);
        }
    }
}
