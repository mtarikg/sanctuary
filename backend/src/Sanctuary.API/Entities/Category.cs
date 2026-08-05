using Sanctuary.API.Entities;
using System;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Note> Notes { get; set; } = [];
}
