using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.Features.Auth;

public record RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50, ErrorMessage = "Username must not exceed 50 characters.")]
    public string Username { get; init; } = string.Empty;
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; init; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; init; } = string.Empty;
}
