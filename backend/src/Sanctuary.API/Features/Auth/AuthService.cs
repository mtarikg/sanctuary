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

public class AuthService(AppDbContext dbContext, IPasswordHasher<User> hasher, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null) return null;

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed) return null;

        var token = GenerateToken(user);

        return new AuthResponse { UserId = user.Id, Username = user.Username, Token = token };
    }

    private string GenerateToken(User user)
    {
        var jwt = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.ExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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

        var token = GenerateToken(user);

        return new AuthResponse { UserId = user.Id, Username = user.Username, Token = token };
    }
}
