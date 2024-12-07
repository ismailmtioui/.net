using System.Security.Claims;

namespace Xyz.SDK.Jwt;

public interface IJwtPayload
{
    bool IsValid { get; }
    void SetFromClaims(IReadOnlyCollection<Claim>? claims);
    IReadOnlyCollection<Claim> ToClaims();
}