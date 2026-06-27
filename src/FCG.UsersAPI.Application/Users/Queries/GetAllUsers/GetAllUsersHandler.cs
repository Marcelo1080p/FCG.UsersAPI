using FCG.UsersAPI.Application.Common;
using FCG.UsersAPI.Domain.Interfaces;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Queries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepo;
    public GetAllUsersHandler(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken ct)
    {
        var users = await _userRepo.GetAllAsync(ct);

        var dtos = users
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role.ToString(), u.IsActive, u.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<UserDto>>.Ok(dtos);
    }
}
