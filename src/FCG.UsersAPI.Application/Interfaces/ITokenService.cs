using FCG.UsersAPI.Domain.Entities;

namespace FCG.UsersAPI.Application.Interfaces;

public interface ITokenService
{
    string Generate(User user);
}
