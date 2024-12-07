using Microsoft.EntityFrameworkCore;
using Xyz.Infrastructure.Abstractions;
using Xyz.Models;
using Xyz.SDK.Dao.EntityFramework;

namespace Xyz.Infrastructure.EF.UserTokens;

internal class UserTokenRepository : RepositoryBase<UnitOfWork, UserToken, int>, IUserTokenRepository
{
    public UserTokenRepository(UnitOfWork context) : base(context)
    {
    }
    
    public async Task DeleteUserTokensAsync(int userId)
    {
        const string command = "DELETE FROM UserToken WHERE UserId = @userId";
        await Context.Database.ExecuteSqlRawAsync(command, new { userId });
    }
    
    public async Task DeleteUserTokensAsync(int userId, int duration)
    {
        const string command = "DELETE FROM UserToken WHERE UserId = @userId AND IssuedAt < DATEADD(SECOND, -@duration, GETUTCDATE())";
        await Context.Database.ExecuteSqlRawAsync(command, new { userId }, new { duration });
    }
}