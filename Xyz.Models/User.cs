using Xyz.SDK.Domain;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Xyz.Models;

public class User : TrackedEntity<int>
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
}