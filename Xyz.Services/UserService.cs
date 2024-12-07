using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xyz.Infrastructure.Abstractions;
using Xyz.Models;
using Xyz.SDK.Dao;
using Xyz.SDK.Service;
using Xyz.SDK.Tools;
using Xyz.Services.Abstractions;

namespace Xyz.Services;

internal class UserService : ServiceBase, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<User> _userValidator;
    
    public UserService(IUnitOfWork uow, ILogger<ServiceBase> logger, IUserRepository userRepository, IValidator<User> userValidator) : base(uow, logger)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
    }

    public async Task<bool> EmailExistsAsync(string email) => 
        await _userRepository.GetUserByEmailAsync(email) is not null;

    public async Task<UserStatus> CreateUserAsync(User user)
    {
        if (!Validate(_userValidator, user))
            return UserStatus.BadInput;
        
        if (await EmailExistsAsync(user.Email))
            return UserStatus.Conflict;
        
        user.PasswordSalt = HashGenerator.GenerateRandomKey();
        user.Password = HashGenerator.ComputeSha256Hash(user.PasswordSalt, user.Password);
        user.CreatedOn = DateTime.UtcNow;
        
        await _userRepository.InsertAsync(user);

        return await Uow.CommitAsync() ? UserStatus.Success : UserStatus.Failed;
    }
    
    public async Task<(UserStatus, User)> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetAll().SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return (UserStatus.NotFound, new User());

        return (UserStatus.Success, user);
    }
    
    public async Task<UserStatus> UpdatePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        if (oldPassword == newPassword)
            return UserStatus.Conflict;
        
        var user = await _userRepository.GetAsync(userId);
        if (user is null)
            return UserStatus.NotFound;
        
        var passwordHashed = HashGenerator.ComputeSha256Hash(user.PasswordSalt, oldPassword);

        if (passwordHashed != user.Password)
            return UserStatus.Forbidden;
        
        return await UpdateUserPasswordAsync(user, newPassword);
    }
    
    private async Task<UserStatus> UpdateUserPasswordAsync(User user, string newPassword)
    {
        user.PasswordSalt = HashGenerator.GenerateRandomKey();
        user.Password = HashGenerator.ComputeSha256Hash(user.PasswordSalt, newPassword);

        return await UpdateUserInfoAsync(user);
    }
    
    private async Task<UserStatus> UpdateUserInfoAsync(User user)
    {
        _userRepository.Update(user);
        return await Uow.CommitAsync() ? UserStatus.Success : UserStatus.Failed;
    }
}