using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Auth;
using Sanctuary.API.Features.Auth.DTOs;

namespace Sanctuary.UnitTests.Features.Auth;

// Note: Extracting into an abstract class could cause leaky tests due to shared state in the mocks.

public abstract class AuthServiceTestsBase
{
    protected readonly Mock<IPasswordHasher<User>> HasherMock = new();
    protected readonly Mock<ITokenService> TokenServiceMock = new();

    protected static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    protected AuthService CreateSut(AppDbContext db) =>
        new(db, HasherMock.Object, TokenServiceMock.Object);
}

[Trait("Category", "Unit")]
public class AuthService_RegisterTests : AuthServiceTestsBase
{
    private void SetupSuccessMocks()
    {
        HasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "password123"))
            .Returns("hashed");
        TokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("token");
    }

    private static RegisterRequest NewUserRequest() => new()
    {
        Email = "new@example.com",
        Username = "newuser",
        Password = "password123"
    };

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNull()
    {
        await using var db = CreateDbContext();
        db.Users.Add(new User { Email = "taken@example.com", Username = "existing", PasswordHash = "x" });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await CreateSut(db).RegisterAsync(new RegisterRequest
        {
            Email = "taken@example.com",
            Username = "newuser",
            Password = "password123"
        }, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsNew_PersistsUserToDatabase()
    {
        await using var db = CreateDbContext();
        SetupSuccessMocks();

        await CreateSut(db).RegisterAsync(NewUserRequest(), CancellationToken.None);

        db.Users.Should().ContainSingle(u => u.Email == "new@example.com");
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsNew_StoresHashedPassword()
    {
        await using var db = CreateDbContext();
        SetupSuccessMocks();

        await CreateSut(db).RegisterAsync(NewUserRequest(), CancellationToken.None);

        db.Users.Should().ContainSingle(u => u.PasswordHash == "hashed");
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsNew_ReturnsAuthResponse()
    {
        await using var db = CreateDbContext();
        SetupSuccessMocks();

        var result = await CreateSut(db).RegisterAsync(NewUserRequest(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
        result.Token.Should().Be("token");
    }
}

[Trait("Category", "Unit")]
public class AuthService_LoginTests : AuthServiceTestsBase
{
    private static User ExistingUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "user@example.com",
        Username = "testuser",
        PasswordHash = "hashed"
    };

    private static LoginRequest ValidRequest(User user) => new()
    {
        Email = user.Email,
        Password = "password123"
    };

    private static async Task<User> SeedUserAsync(AppDbContext db)
    {
        var user = ExistingUser();
        db.Users.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var result = await CreateSut(db).LoginAsync(new LoginRequest
        {
            Email = "ghost@example.com",
            Password = "password123"
        }, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordFails_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var user = await SeedUserAsync(db);

        HasherMock
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), "hashed", "wrongpassword"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await CreateSut(db).LoginAsync(new LoginRequest
        {
            Email = user.Email,
            Password = "wrongpassword"
        }, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsAuthResponse()
    {
        await using var db = CreateDbContext();
        var user = await SeedUserAsync(db);

        HasherMock
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), "hashed", "password123"))
            .Returns(PasswordVerificationResult.Success);
        TokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("token");

        var result = await CreateSut(db).LoginAsync(ValidRequest(user), CancellationToken.None);

        result.Should().NotBeNull();
        result.Username.Should().Be(user.Username);
        result.Token.Should().Be("token");
    }
}