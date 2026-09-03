using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.Features.Auth.DTOs;

public record LoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = string.Empty;
}
