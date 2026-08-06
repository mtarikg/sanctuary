namespace Sanctuary.API.DTOs;

public record NoteResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int UserId { get; init; }
    public CategoryResponse? Category { get; init; }
}
