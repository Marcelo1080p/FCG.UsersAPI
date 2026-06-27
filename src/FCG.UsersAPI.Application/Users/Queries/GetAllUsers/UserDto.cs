namespace FCG.UsersAPI.Application.Users.Queries.GetAllUsers;

public record UserDto(Guid Id, string Name, string Email, string Role, bool IsActive, DateTime CreatedAt);
