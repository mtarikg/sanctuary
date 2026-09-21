using Sanctuary.API.Features.Auth.DTOs;
using Sanctuary.API.Infrastructure.Decorators;
using System.Security.Cryptography;
using System.Text;

namespace Sanctuary.API.Features.Auth;

public partial class LoggingAuthServiceDecorator
{
    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} registered")]
    private static partial void LogUserRegistered(ILogger logger, Guid userId);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Registration failed for {EmailHash}")]
    private static partial void LogRegistrationFailed(ILogger logger, string emailHash);
    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} logged in")]
    private static partial void LogUserLoggedIn(ILogger logger, Guid userId);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Login failed for {EmailHash}")]
    private static partial void LogLoginFailed(ILogger logger, string emailHash);

}

public partial class LoggingAuthServiceDecorator(IAuthService inner, ILogger<LoggingAuthServiceDecorator> logger) : LoggingDecorator(logger), IAuthService
{
    public Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        return ExecuteWithLoggingAsync(
            () => inner.LoginAsync(request, ct),
            result => LogUserLoggedIn(Logger, result.UserId),
            () => LogLoginFailed(Logger, HashForLog(request.Email)));
    }

    public Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        return ExecuteWithLoggingAsync(
            () => inner.RegisterAsync(request, ct),
            result => LogUserRegistered(Logger, result.UserId),
            () => LogRegistrationFailed(Logger, HashForLog(request.Email)));
    }

    private static string HashForLog(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes)[..8];
    }
}
