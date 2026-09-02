using Sanctuary.API.Features.Categories.DTOs;

namespace Sanctuary.API.Features.Notes.DTOs;

public record NoteResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public CategoryResponse? Category { get; init; }
}
