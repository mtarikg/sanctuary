using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanctuary.API.Data;
using Sanctuary.API.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sanctuary.API.Features.Auth;

public class AuthService(AppDbContext dbContext, IPasswordHasher<User> hasher, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null) return null;

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed) return null;

        return new AuthResponse { UserId = user.Id, Username = user.Username, Token = tokenService.GenerateToken(user) };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var userExists = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email);

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
