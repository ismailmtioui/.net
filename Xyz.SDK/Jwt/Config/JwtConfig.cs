#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Xyz.SDK.Jwt.Config;

public class JwtConfig
{
    public string Key { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int TokenExpiresIn { get; set; }
    public int RecoveryTokenExpiresIn { get; set; }
}