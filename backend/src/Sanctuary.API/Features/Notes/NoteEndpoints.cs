using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Constants;
using System.Security.Claims;

namespace Sanctuary.API.Features.Notes;

public static class NoteEndpoints
{
    public static void MapNoteEndPoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroups.Notes).RequireAuthorization();

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);

    }

    private static Guid GetUserId(HttpContext context) => Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User ID claim missing."));

    private static async Task<IResult> Delete(HttpContext context, INoteService noteService, Guid id, CancellationToken ct)
    {
        var deleted = await noteService.DeleteAsync(GetUserId(context), id, ct);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> Update(HttpContext context, INoteService noteService, Guid id, UpdateNoteRequest request, CancellationToken ct)
    {
        var note = await noteService.UpdateAsync(GetUserId(context), id, request, ct);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> Create(HttpContext context, INoteService noteService, CreateNoteRequest request, CancellationToken ct)
    {
        var note = await noteService.CreateAsync(GetUserId(context), request, ct);
        return TypedResults.Created($"/notes/{note.Id}", note);
    }

    private static async Task<IResult> GetById(HttpContext context, INoteService noteService, Guid id, CancellationToken ct)
    {
        var note = await noteService.GetByIdAsync(GetUserId(context), id, ct);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> GetAll(HttpContext context, INoteService noteService, CancellationToken ct)
    {
        var notes = await noteService.GetAllAsync(GetUserId(context), ct);
        return TypedResults.Ok(notes);
    }
}
