using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xyz.SDK.Jwt.Config;
using Xyz.SDK.Jwt.Model;

namespace Xyz.SDK.Jwt;

internal class JwtHandler : IJwtHandler
{
    private readonly JwtConfig _config;
    private readonly ILogger _logger;
    public JwtHandler(IOptions<JwtConfig> config, ILogger<JwtHandler> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public JwtConfig Config => _config;

    public string EncodeToken<T>(T payload, int? expiresIn = null)
        where T : class, IJwtPayload, new()
    {
        try {
            var token = new JwtSecurityToken(
                issuer : _config.Issuer,
                audience: _config.Audience,
                claims: payload.ToClaims(),
                expires:  DateTime.UtcNow.AddSeconds(expiresIn ?? _config.TokenExpiresIn),
                signingCredentials: GetJwtHandlerCredentials(_config.Key)
            );
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        } catch (Exception exception) {
            _logger.Log(LogLevel.Error, exception, $"Error encoding JWT token with payload: {JsonSerializer.Serialize(payload)}");
        }
        return string.Empty;
    }

    public JwtResponse<T> DecodeToken<T>(string token) where T : class, IJwtPayload, new()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }
                , out _);

            if (claimsPrincipal?.Claims.Any() != true)
                return JwtResponse<T>.FromStatus(JwtStatus.InvalidToken);
            
            var payload = new T();
            payload.SetFromClaims(claimsPrincipal.Claims.ToArray());

            return JwtResponse<T>.FromPayload(payload);
        }
        catch (SecurityTokenExpiredException)
        {
            var jwtToken = tokenHandler.ReadToken(token);
            var payload = new T();
            payload.SetFromClaims((jwtToken as JwtSecurityToken)?.Claims.ToArray());
            var result = JwtResponse<T>.FromStatus(JwtStatus.ExpiredToken);
            result.Payload = payload;
            return result;
        }
        catch (Exception exception)
        {
            if (exception is ArgumentException && exception.Message.Contains("IDX12741"))
            {
                _logger.Log(LogLevel.Warning, exception, $"Jwt {token} malformed: {token}");
            }
            else
            {
                _logger.Log(LogLevel.Warning, exception, $"Jwt {token} can not be decoded: {token}");
            }               
            return JwtResponse<T>.FromStatus(JwtStatus.InvalidToken);
        }
    }
    
    private static SigningCredentials GetJwtHandlerCredentials(string key)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        return new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);    
    }
}