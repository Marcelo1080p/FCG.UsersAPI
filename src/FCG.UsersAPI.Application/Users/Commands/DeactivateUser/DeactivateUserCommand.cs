using FCG.UsersAPI.Application.Common;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : IRequest<Result<Guid>>;
