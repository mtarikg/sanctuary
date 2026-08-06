using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.DTOs;
using Sanctuary.API.Entities;

namespace Sanctuary.API.Services;

public class NoteService(AppDbContext dbContext) : INoteService
{
    public async Task<NoteResponse> CreateAsync(int userId, CreateNoteRequest request)
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
        await dbContext.SaveChangesAsync();

        note = await dbContext.Notes
            .Include(n => n.Category)
            .FirstAsync(n => n.Id == note.Id);

        return ToResponse(note);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var note = await dbContext.Notes
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id);

        if (note is null) return false;

        dbContext.Notes.Remove(note);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<NoteResponse>> GetAllAsync(int userId)
    {
        return await dbContext.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.Category)
            .Select(n => ToResponse(n))
            .ToListAsync();
    }

    public async Task<NoteResponse?> GetByIdAsync(int id, int userId)
    {
        var note = await dbContext.Notes
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id);

        return note is null ? null : ToResponse(note);
    }

    public async Task<NoteResponse?> UpdateAsync(int id, int userId, UpdateNoteRequest request)
    {
        var note = await dbContext.Notes
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id);

        if (note is null) return null;

        bool categoryChanged = note.CategoryId != request.CategoryId;

        note.Title = request.Title;
        note.Body = request.Body;
        note.CategoryId = request.CategoryId;
        note.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        if (categoryChanged)
        {
            await dbContext.Entry(note).Reference(n => n.Category).LoadAsync();
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
        UserId = note.UserId,
        Category = note.Category is null ? null : new CategoryResponse
        {
            Id = note.Category.Id,
            Name = note.Category.Name
        }
    };
}
