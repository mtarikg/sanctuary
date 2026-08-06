using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.DTOs;

public record UpdateNoteRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    public string Body { get; init; } = string.Empty;

    public int? CategoryId { get; init; }
}