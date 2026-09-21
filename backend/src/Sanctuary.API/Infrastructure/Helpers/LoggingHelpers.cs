using System.Security.Cryptography;
using System.Text;

namespace Sanctuary.API.Infrastructure.Helpers;

public static class LoggingHelpers
{
    public static string HashForLog(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes)[..8];
    }
}
