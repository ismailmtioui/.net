
using Xyz.Models;

namespace Xyz.Services.Abstractions;

public interface IUserService
{
    Task<bool> EmailExistsAsync(string email);
    Task<UserStatus> CreateUserAsync(User user);
    Task<(UserStatus, User)> GetUserByIdAsync(int userId);
    Task<UserStatus> UpdatePasswordAsync(int userId, string oldPassword, string newPassword);
}