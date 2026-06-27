using MediatR;
using FCG.UsersAPI.Application.Common;

namespace FCG.UsersAPI.Application.Users.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;
