using BackendApi.Data;
using BackendApi.DTOs.Auth;
using BackendApi.Entities.Auth;
using BackendApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Auth;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext    _context;

    public UserService(IUserRepository userRepository, AppDbContext context)
    {
        _userRepository = userRepository;
        _context        = context;
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            return false;

        var user = new User
        {
            Email        = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        await _userRepository.AddAsync(user);

        _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = 6 });
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IList<string>> GetUserRolesAsync(int userId) =>
        await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    
    public async Task<IList<UserLookupDto>> GetLookupAsync() =>
    await (from u in _context.Users
           join t in _context.Teams on u.TeamId equals t.Id into teamJoin
           from t in teamJoin.DefaultIfEmpty()
           where !u.IsDeleted
           select new UserLookupDto(u.Id, u.Email, t != null ? t.Name : null))
          .ToListAsync();

}