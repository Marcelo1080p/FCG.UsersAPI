using FCG.UsersAPI.Application.Common;
using FCG.UsersAPI.Domain.Interfaces;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepo;
    public DeactivateUserCommandHandler(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<Result<Guid>> Handle(DeactivateUserCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId, ct);
        if (user is null)
            return Result<Guid>.Fail("Usuário não encontrado.");

        if (!user.IsActive)
            return Result<Guid>.Fail("Usuário já está inativo.");

        user.Deactivate();
        await _userRepo.UpdateAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);

        return Result<Guid>.Ok(user.Id);
    }
}
