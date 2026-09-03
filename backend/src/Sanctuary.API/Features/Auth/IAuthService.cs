using Sanctuary.API.Features.Auth.DTOs;

namespace Sanctuary.API.Features.Auth;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
