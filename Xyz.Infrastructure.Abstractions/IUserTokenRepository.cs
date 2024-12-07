using Xyz.Models;
using Xyz.SDK.Dao;

namespace Xyz.Infrastructure.Abstractions;

public interface IUserTokenRepository : IRepository<UserToken, int>
{
    Task DeleteUserTokensAsync(int userId);
}