using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.Infrastructure.Settings;

public record JwtSettings
{
    [Required]
    public string SecretKey { get; init; } = string.Empty;
    [Required]
    public string Issuer { get; init; } = string.Empty;
    [Required]
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 60;
}
