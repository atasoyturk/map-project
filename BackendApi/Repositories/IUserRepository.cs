using BackendApi.Entities.Auth;

namespace BackendApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);  
    Task AddAsync(User user);     
}