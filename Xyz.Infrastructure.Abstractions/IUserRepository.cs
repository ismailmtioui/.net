using Xyz.Models;
using Xyz.SDK.Dao;

namespace Xyz.Infrastructure.Abstractions;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetUserByEmailAsync(string email);
}