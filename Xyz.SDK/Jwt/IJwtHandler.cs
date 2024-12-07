using Xyz.SDK.Jwt.Config;
using Xyz.SDK.Jwt.Model;

namespace Xyz.SDK.Jwt;

public interface IJwtHandler
{
    JwtConfig Config { get; }
    string EncodeToken<T>(T payload, int? expiresIn = null) where T : class, IJwtPayload, new();
    JwtResponse<T> DecodeToken<T>(string token) where T : class, IJwtPayload, new();
}