using FCG.UsersAPI.Domain.Entities;
using FCG.UsersAPI.Domain.Interfaces;
using FCG.UsersAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.UsersAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _ctx;

    public UserRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _ctx.Users.FirstOrDefaultAsync(
            u => u.Email == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsAsync(string email, CancellationToken ct = default)
        => _ctx.Users.AnyAsync(
            u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Users.ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _ctx.Users.AddAsync(user, ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _ctx.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _ctx.SaveChangesAsync(ct);
}
