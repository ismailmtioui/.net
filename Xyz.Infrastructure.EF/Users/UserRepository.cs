using Microsoft.EntityFrameworkCore;
using Xyz.Infrastructure.Abstractions;
using Xyz.Models;
using Xyz.SDK.Dao.EntityFramework;

namespace Xyz.Infrastructure.EF.Users;

internal class UserRepository : RepositoryBase<UnitOfWork, User, int>, IUserRepository
{
    public UserRepository(UnitOfWork context) : base(context)
    {
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Set.SingleOrDefaultAsync(c => c.Email == email);
    }
}