using MediatR;
using FCG.UsersAPI.Application.Common;

namespace FCG.UsersAPI.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<Result<Guid>>;
