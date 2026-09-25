using FluentAssertions;
using Sanctuary.API.Features.Auth.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace Sanctuary.IntegrationTests.Features.Auth;

[Trait("Category", "Integration")]
public class AuthEndpointTests : IntegrationTestBase
{
    private static readonly object ValidRegisterRequest = new
    {
        email = "test@example.com",
        username = "testuser",
        password = "Test1234!"
    };

    private async Task RegisterDefaultUserAsync() =>
        await Client.PostAsJsonAsync("/auth/register", ValidRegisterRequest, CancellationToken.None);

    [Fact]
    public async Task Register_WithValidRequest_Returns200()
    {
        var response = await Client.PostAsJsonAsync("/auth/register", ValidRegisterRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsAuthResponse()
    {
        var response = await Client.PostAsJsonAsync("/auth/register", ValidRegisterRequest, CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(CancellationToken.None);

        body.Should().NotBeNull();
        body.Username.Should().Be("testuser");
        body.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        await Client.PostAsJsonAsync("/auth/register", ValidRegisterRequest, CancellationToken.None);
        var response = await Client.PostAsJsonAsync("/auth/register", ValidRegisterRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidRequest_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/auth/register", new
        {
            email = "notanemail",
            username = "",
            password = ""
        }, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200()
    {
        await RegisterDefaultUserAsync();

        var response = await Client.PostAsJsonAsync("/auth/login", new
        {
            email = "test@example.com",
            password = "Test1234!"
        }, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAuthResponse()
    {
        await RegisterDefaultUserAsync();

        var response = await Client.PostAsJsonAsync("/auth/login", new
        {
            email = "test@example.com",
            password = "Test1234!"
        }, CancellationToken.None);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(CancellationToken.None);

        body.Should().NotBeNull();
        body.Username.Should().Be("testuser");
        body.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        await RegisterDefaultUserAsync();

        var response = await Client.PostAsJsonAsync("/auth/login", new
        {
            email = "test@example.com",
            password = "wrongpassword"
        }, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/auth/login", new
        {
            email = "ghost@example.com",
            password = "Test1234!"
        }, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidRequest_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/auth/login", new
        {
            email = "notanemail",
            password = ""
        }, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
