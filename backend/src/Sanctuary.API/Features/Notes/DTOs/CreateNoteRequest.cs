using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.Features.Notes.DTOs;

public record CreateNoteRequest
{
    [Required(ErrorMessage = "Please provide a title for your note.")]
    [MaxLength(200, ErrorMessage = "The note title cannot exceed 200 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "The note body cannot be blank.")]
    public string Body { get; init; } = string.Empty;

    public int? CategoryId { get; init; }
}
