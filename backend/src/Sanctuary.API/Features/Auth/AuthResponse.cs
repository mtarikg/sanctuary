namespace Sanctuary.API.Features.Auth;

public record AuthResponse
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}