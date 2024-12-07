using FluentValidation;
using Xyz.Infrastructure.Abstractions;
using Xyz.Models;

namespace Xyz.Services.Validators;

public class UserValidator : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;
    public UserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(user => user.Email)
            .CustomAsync(async (email, context, cancellation) =>
            {
                if (!await IsEmailExistsAsync(email))
                {
                    context.AddFailure("Email not found in DB.");
                }
            });
    }
    
    private async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email) is not null;
    } 
}