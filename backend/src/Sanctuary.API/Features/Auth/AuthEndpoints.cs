namespace Sanctuary.API.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");
        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
    }

    private static async Task<IResult> Login(IAuthService authService, LoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        return result is null ? TypedResults.Unauthorized() : TypedResults.Ok(result);
    }

    private static async Task<IResult> Register(IAuthService authService, RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        return result is null ? TypedResults.Conflict("Email already in use") : TypedResults.Ok(result);
    }
}
