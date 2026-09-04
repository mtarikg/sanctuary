using Sanctuary.API.Features.Auth.DTOs;
using Sanctuary.API.Infrastructure.Constants;

namespace Sanctuary.API.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroups.Auth).RequireRateLimiting(RateLimitingPolicies.Auth);
        group.MapPost(AuthRoutes.Register, Register);
        group.MapPost(AuthRoutes.Login, Login);
    }

    private static async Task<IResult> Login(IAuthService authService, LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);

        return result is null ? TypedResults.Unauthorized() : TypedResults.Ok(result);
    }

    private static async Task<IResult> Register(IAuthService authService, RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);

        return result is null ? TypedResults.Conflict("Email already in use") : TypedResults.Ok(result);
    }
}
