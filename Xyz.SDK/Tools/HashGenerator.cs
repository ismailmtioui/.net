using System.Security.Cryptography;
using System.Text;

namespace Xyz.SDK.Tools;

public class HashGenerator
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int KeyLength = 32;

    public static string GenerateRandomKey()
    {
        var random = new Random();
        var key = new char[KeyLength];
        for (int i = 0; i < KeyLength; i++)
        {
            key[i] = AllowedChars[random.Next(AllowedChars.Length)];
        }
        return new string(key);
    }

    public static string ComputeSha256Hash(string key, string input)
    {
        using var sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes($"{key}{input}"));

        // Convert byte array to a string
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}