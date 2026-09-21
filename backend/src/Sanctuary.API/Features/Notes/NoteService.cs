using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Entities;
using Sanctuary.API.Features.Categories.DTOs;
using Sanctuary.API.Features.Notes.DTOs;

namespace Sanctuary.API.Features.Notes;

public class NoteService(AppDbContext dbContext) : INoteService
{
    public async Task<NoteResponse> CreateAsync(Guid userId, CreateNoteRequest request, CancellationToken ct = default)
    {
        var note = new Note
        {
            Title = request.Title,
            Body = request.Body,
            CreatedAt = DateTime.UtcNow,
            CategoryId = request.CategoryId,
            UserId = userId
        };

        dbContext.Notes.Add(note);
        await dbContext.SaveChangesAsync(ct);

        note = await dbContext.Notes
            .Include(n => n.Category)
            .FirstAsync(n => n.Id == note.Id, ct);

        return ToResponse(note);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var note = await dbContext.Notes
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id, ct);

        if (note is null) return false;

        dbContext.Notes.Remove(note);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<IEnumerable<NoteResponse>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        return await dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .Include(n => n.Category)
            .Select(n => ToResponse(n))
            .ToListAsync(ct);
    }

    public async Task<NoteResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var note = await dbContext.Notes
            .AsNoTracking()
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id, ct);

        return note is null ? null : ToResponse(note);
    }

    public async Task<NoteResponse?> UpdateAsync(Guid userId, Guid id, UpdateNoteRequest request, CancellationToken ct = default)
    {
        var note = await dbContext.Notes
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id, ct);

        if (note is null) return null;

        bool categoryChanged = note.CategoryId != request.CategoryId;

        note.Title = request.Title;
        note.Body = request.Body;
        note.CategoryId = request.CategoryId;
        note.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        if (categoryChanged)
        {
            await dbContext.Entry(note).Reference(n => n.Category).LoadAsync(ct);
        }

        return ToResponse(note);
    }

    private static NoteResponse ToResponse(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Body = note.Body,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt,
        Category = note.Category is null ? null : new CategoryResponse
        {
            Id = note.Category.Id,
            Name = note.Category.Name
        }
    };
}
