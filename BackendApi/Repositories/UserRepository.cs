using Microsoft.EntityFrameworkCore;

using BackendApi.Data;
using BackendApi.Entities;

namespace BackendApi.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email); // email never added to query with string concatenation
}