namespace Sanctuary.API.Features.Categories.DTOs;

public record CategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}