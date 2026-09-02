using System;

namespace Sanctuary.API.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}