using FluentAssertions;
using Microsoft.Extensions.Options;
using Sanctuary.API.Features.Auth;
using Sanctuary.API.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sanctuary.UnitTests.Features.Auth;

[Trait("Category", "Unit")]
public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings = new()
    {
        SecretKey = "super-secret-key-that-is-long-enough-for-hmac",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpiryMinutes = 60
    };

    private TokenService CreateSut() => new(Options.Create(_jwtSettings)); // system under test

    private static User CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Username = "testuser",
        Email = "test@example.com",
        PasswordHash = "irrelevant"
    };

    private JwtSecurityToken GenerateAndParse(User user)
    {
        var token = CreateSut().GenerateToken(user);
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return parsed;
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyString()
    {
        var token = CreateSut().GenerateToken(CreateUser());
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_ReturnsValidJwt()
    {
        var token = CreateSut().GenerateToken(CreateUser());
        var handler = new JwtSecurityTokenHandler();

        handler.CanReadToken(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ContainsCorrectNameIdentifierClaim()
    {
        var user = CreateUser();
        var parsed = GenerateAndParse(user);

        parsed.Claims
            .First(c => c.Type == ClaimTypes.NameIdentifier)
            .Value
            .Should().Be(user.Id.ToString());
    }

    [Fact]
    public void GenerateToken_ContainsCorrectUsernameClaim()
    {
        var user = CreateUser();
        var parsed = GenerateAndParse(user);

        parsed.Claims
            .First(c => c.Type == ClaimTypes.Name)
            .Value
            .Should().Be(user.Username);
    }

    [Fact]
    public void GenerateToken_HasCorrectIssuer()
    {
        GenerateAndParse(CreateUser()).Issuer.Should().Be(_jwtSettings.Issuer);
    }

    [Fact]
    public void GenerateToken_HasCorrectAudience()
    {
        GenerateAndParse(CreateUser()).Audiences.Should().Contain(_jwtSettings.Audience);
    }

    [Fact]
    public void GenerateToken_ExpiryIsApproximatelyCorrect()
    {
        var before = DateTime.UtcNow;
        var parsed = GenerateAndParse(CreateUser());

        parsed.ValidTo.Should().BeCloseTo(
            before.AddMinutes(_jwtSettings.ExpiryMinutes),
            precision: TimeSpan.FromSeconds(5));
    }

}
