using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Entities;
using Sanctuary.API.Features.Notes;
using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Common;

namespace Sanctuary.UnitTests.Features.Notes;

// Note: UpdateAsync category-changed branch (Reference.LoadAsync) is not covered here.
// In-memory EF does not support navigation reference loading.
// Covered in integration tests.

[Trait("Category", "Unit")]
public class NoteServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static NoteService CreateSut(AppDbContext dbContext) => new(dbContext);

    private static Note CreateNote(Guid userId, int? categoryId = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Test Note",
        Body = "Test Body",
        CreatedAt = DateTime.UtcNow,
        UserId = userId,
        CategoryId = categoryId
    };

    private static async Task<(Guid userId, Note note)> SeedNoteAsync(AppDbContext db)
    {
        var userId = Guid.NewGuid();
        var note = CreateNote(userId);
        db.Notes.Add(note);
        await db.SaveChangesAsync(CancellationToken.None);

        return (userId, note);
    }

    private static CreateNoteRequest CreateRequest() => new()
    {
        Title = "New Note",
        Body = "Body",
        CategoryId = null
    };

    private static UpdateNoteRequest UpdateRequest() => new()
    {
        Title = "Updated Title",
        Body = "Updated Body",
        CategoryId = null
    };

    private static PaginationParams DefaultPagination() => new() { Page = 1, PageSize = 10 };

    [Fact]
    public async Task CreateAsync_PersistsNoteToDatabase()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();

        await CreateSut(db).CreateAsync(userId, CreateRequest(), CancellationToken.None);

        db.Notes.Should().ContainSingle(n => n.UserId == userId);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNoteResponse()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();

        var result = await CreateSut(db).CreateAsync(userId, CreateRequest(), CancellationToken.None);

        result.Title.Should().Be("New Note");
        result.Body.Should().Be("Body");
    }

    [Fact]
    public async Task DeleteAsync_WhenNoteNotFound_ReturnsFalse()
    {
        await using var db = CreateDbContext();

        var result = await CreateSut(db).DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenNoteExistsForUser_ReturnsTrue()
    {
        await using var db = CreateDbContext();
        var (userId, note) = await SeedNoteAsync(db);

        var result = await CreateSut(db).DeleteAsync(userId, note.Id, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenNoteExistsForUser_RemovesFromDatabase()
    {
        await using var db = CreateDbContext();
        var (userId, note) = await SeedNoteAsync(db);

        await CreateSut(db).DeleteAsync(userId, note.Id, CancellationToken.None);
        db.Notes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasNoNotes_ReturnsEmpty()
    {
        await using var db = CreateDbContext();

        var result = await CreateSut(db).GetAllAsync(Guid.NewGuid(), DefaultPagination(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginationMetadata()
    {
        await using var db = CreateDbContext();
        var userId = Guid.NewGuid();
        db.Notes.AddRange(CreateNote(userId), CreateNote(userId), CreateNote(userId));
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await CreateSut(db).GetAllAsync(userId, new PaginationParams
        {
            Page = 1,
            PageSize = 2
        }, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoteNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var result = await CreateSut(db).GetByIdAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoteExistsForUser_ReturnsNoteResponse()
    {
        await using var db = CreateDbContext();
        var (userId, note) = await SeedNoteAsync(db);

        var result = await CreateSut(db).GetByIdAsync(userId, note.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(note.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenNoteNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var result = await CreateSut(db).UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), UpdateRequest(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenNoteExists_ReturnsUpdatedResponse()
    {
        await using var db = CreateDbContext();
        var (userId, note) = await SeedNoteAsync(db);

        var result = await CreateSut(db).UpdateAsync(userId, note.Id, UpdateRequest(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task UpdateAsync_WhenNoteExists_PersistsChangesToDatabase()
    {
        await using var db = CreateDbContext();
        var (userId, note) = await SeedNoteAsync(db);

        await CreateSut(db).UpdateAsync(userId, note.Id, UpdateRequest(), CancellationToken.None);

        var persisted = await db.Notes.FirstAsync(n => n.Id == note.Id, CancellationToken.None);
        persisted.Title.Should().Be("Updated Title");
        persisted.Body.Should().Be("Updated Body");
    }
}
