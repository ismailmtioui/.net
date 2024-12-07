using Xyz.SDK.Domain;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Xyz.Models;

public class UserToken : EntityBase<int>
{
    public TokenProvider TokenProvider { get; set; }
    public string? UserReferenceId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Token { get; set; }
    public DateTime IssuedAt { get; set; }
}