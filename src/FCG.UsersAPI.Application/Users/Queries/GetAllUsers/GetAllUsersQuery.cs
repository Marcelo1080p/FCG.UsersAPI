using FCG.UsersAPI.Application.Common;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserDto>>>;
