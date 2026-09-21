using Sanctuary.API.Entities;
using Sanctuary.API.Features.Categories.DTOs;
using Sanctuary.API.Features.Notes.DTOs;

namespace Sanctuary.API.Features.Notes;

public static class NoteMapper
{
    public static NoteResponse ToResponse(Note note) => new()
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
