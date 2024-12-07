#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Xyz.SDK.Jwt.Model;

public class JwtResponse<T> where T : new()
{
    public T Payload { get; set; }
    public JwtStatus Status { get; private set; }
 
    public static JwtResponse<T> FromStatus(JwtStatus status)
    {
        return new JwtResponse<T> { Status = status, Payload = new T()};
    }
 
    public static JwtResponse<T> FromPayload(T payload)
    {
        return new JwtResponse<T> { Payload = payload, Status = JwtStatus.Success };
    }
}