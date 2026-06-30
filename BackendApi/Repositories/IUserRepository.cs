using BackendApi.Entities;

namespace BackendApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}