using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Auth.DTOs;

namespace Sanctuary.API.Features.Auth;

public class AuthService(AppDbContext dbContext, IPasswordHasher<User> hasher, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user == null) return null;

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed) return null;

        return new AuthResponse { UserId = user.Id, Username = user.Username, Token = tokenService.GenerateToken(user) };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var userExists = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email, ct);

        if (userExists) return null;

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return new AuthResponse { UserId = user.Id, Username = user.Username, Token = tokenService.GenerateToken(user) };
    }
}
