using FCG.UsersAPI.Application.Common;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.PromoteUser;

public record PromoteUserCommand(Guid UserId) : IRequest<Result<Guid>>;
