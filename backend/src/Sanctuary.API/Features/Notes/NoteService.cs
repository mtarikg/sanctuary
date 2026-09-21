using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Entities;
using Sanctuary.API.Features.Categories.DTOs;
using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Common;
using Sanctuary.API.Infrastructure.Constants;

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

        return NoteMapper.ToResponse(note);
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

    public async Task<PagedResult<NoteResponse>> GetAllAsync(Guid userId, PaginationParams pagination, CancellationToken ct = default)
    {
        var query = dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(n => n.Category)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return new PagedResult<NoteResponse>
        {
            Items = items.Select(NoteMapper.ToResponse),
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }

    public async Task<NoteResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var note = await dbContext.Notes
            .AsNoTracking()
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id, ct);

        return note is null ? null : NoteMapper.ToResponse(note);
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

        return NoteMapper.ToResponse(note);
    }
}
