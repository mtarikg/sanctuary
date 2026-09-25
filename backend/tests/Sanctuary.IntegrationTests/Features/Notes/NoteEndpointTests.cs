using FluentAssertions;
using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Common;
using System.Net;
using System.Net.Http.Json;

namespace Sanctuary.IntegrationTests.Features.Notes;

public class NoteEndpointTests : IntegrationTestBase
{
    private static readonly object ValidCreateRequest = new
    {
        title = "Test Note",
        body = "Test Body",
        categoryId = (int?)null
    };

    private static readonly object ValidUpdateRequest = new
    {
        title = "Updated Title",
        body = "Updated Body",
        categoryId = (int?)null
    };

    private static object InvalidRequest => new
    {
        title = (string?)null,
        body = (string?)null,
        categoryId = (int?)null
    };

    private async Task<(HttpClient client, NoteResponse note)> CreateAuthenticatedClientWithNoteAsync()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);
        var note = await response.Content.ReadFromJsonAsync<NoteResponse>(CancellationToken.None);

        return (client, note!);
    }

    [Fact]
    public async Task Notes_WithoutToken_Returns401()
    {
        var response = await Client.GetAsync("/notes", CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithNoNotes_Returns200WithEmptyPagedResult()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/notes", CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<NoteResponse>>(CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Items.Should().BeEmpty();
        body.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAll_WithNotes_Returns200WithPagedResult()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);
        await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);

        var response = await client.GetAsync("/notes", CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<NoteResponse>>(CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.TotalCount.Should().Be(2);
        body.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectPage()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);
        await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);
        await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);

        var response = await client.GetAsync("/notes?page=1&pageSize=2", CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<NoteResponse>>(CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.TotalCount.Should().Be(3);
        body.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WhenNoteExists_Returns200()
    {
        var (client, note) = await CreateAuthenticatedClientWithNoteAsync();

        var response = await client.GetAsync($"/notes/{note.Id}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenNoteNotFound_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/notes/{Guid.NewGuid()}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidRequest_Returns201()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsNoteResponse()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/notes", ValidCreateRequest, CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<NoteResponse>(CancellationToken.None);

        body!.Title.Should().Be("Test Note");
        body.Body.Should().Be("Test Body");
    }

    [Fact]
    public async Task Create_WithInvalidRequest_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/notes", InvalidRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WhenNoteExists_Returns200()
    {
        var (client, note) = await CreateAuthenticatedClientWithNoteAsync();

        var response = await client.PutAsJsonAsync($"/notes/{note.Id}", ValidUpdateRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WhenNoteExists_ReturnsUpdatedNoteResponse()
    {
        var (client, note) = await CreateAuthenticatedClientWithNoteAsync();

        var response = await client.PutAsJsonAsync($"/notes/{note.Id}", ValidUpdateRequest, CancellationToken.None);
        var body = await response.Content.ReadFromJsonAsync<NoteResponse>(CancellationToken.None);

        body!.Title.Should().Be("Updated Title");
        body.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task Update_WhenNoteNotFound_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PutAsJsonAsync($"/notes/{Guid.NewGuid()}", ValidUpdateRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithInvalidRequest_Returns400()
    {
        var (client, note) = await CreateAuthenticatedClientWithNoteAsync();

        var response = await client.PutAsJsonAsync($"/notes/{note.Id}", InvalidRequest, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WhenNoteExists_Returns204()
    {
        var (client, note) = await CreateAuthenticatedClientWithNoteAsync();

        var response = await client.DeleteAsync($"/notes/{note.Id}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenNoteNotFound_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.DeleteAsync($"/notes/{Guid.NewGuid()}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
