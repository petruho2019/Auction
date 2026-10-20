using System.Security.Cryptography;
using System.Text;

namespace Identity.Data;

public static class OpaqueToken
{
    public static string Generate() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
