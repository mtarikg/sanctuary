using Microsoft.AspNetCore.Mvc.Testing;
using Sanctuary.API.Features.Auth.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sanctuary.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly List<HttpClient> _clients = [];

    protected HttpClient Client = null!;
    protected readonly SanctuaryWebAppFactory Factory = new();

    public async ValueTask InitializeAsync()
    {
        await Factory.InitializeAsync();

        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        Client.Timeout = TimeSpan.FromSeconds(30);
        _clients.Add(Client);

        await Factory.ResetDatabaseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients)
            client.Dispose();

        await Factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected async Task<string> GetTokenAsync(
        string email = "test@example.com",
        string username = "testuser",
        string password = "Test1234!")
    {
        var response = await Client.PostAsJsonAsync("/auth/register", new { email, username, password });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return body!.Token;
    }

    protected HttpClient CreateAuthenticatedClient(string token)
    {
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _clients.Add(client);
        return client;
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(
    string email = "test@example.com",
    string username = "testuser",
    string password = "Test1234!")
    {
        var token = await GetTokenAsync(email, username, password);

        return CreateAuthenticatedClient(token);
    }

}