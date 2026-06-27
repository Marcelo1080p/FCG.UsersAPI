using FCG.UsersAPI.Application.Common;
using FCG.UsersAPI.Domain.Enums;
using FCG.UsersAPI.Domain.Interfaces;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.PromoteUser;

public class PromoteUserCommandHandler : IRequestHandler<PromoteUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepo;
    public PromoteUserCommandHandler(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<Result<Guid>> Handle(PromoteUserCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId, ct);
        if (user is null)
            return Result<Guid>.Fail("Usuário não encontrado.");

        if (user.Role == UserRole.Admin)
            return Result<Guid>.Fail("Usuário já é administrador.");

        user.PromoteToAdmin();
        await _userRepo.UpdateAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);

        return Result<Guid>.Ok(user.Id);
    }
}
