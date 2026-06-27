using FCG.UsersAPI.Application.Common;
using FCG.UsersAPI.Application.Interfaces;
using FCG.UsersAPI.Domain.Interfaces;
using MediatR;

namespace FCG.UsersAPI.Application.Users.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUserRepository _repo;
    private readonly ITokenService _tokenService;

    public LoginHandler(IUserRepository repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !user.IsActive)
            return Result<string>.Fail("Credenciais inválidas.");

        if (!user.VerifyPassword(cmd.Password))
            return Result<string>.Fail("Credenciais inválidas.");

        var token = _tokenService.Generate(user);
        return Result<string>.Ok(token);
    }
}
