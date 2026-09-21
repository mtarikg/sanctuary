using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Constants;
using Sanctuary.API.Infrastructure.Extensions;

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

    private static async Task<IResult> Delete(HttpContext context, INoteService noteService, Guid id, CancellationToken ct)
    {
        var deleted = await noteService.DeleteAsync(context.User.GetUserId(), id, ct);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> Update(HttpContext context, INoteService noteService, Guid id, UpdateNoteRequest request, CancellationToken ct)
    {
        var note = await noteService.UpdateAsync(context.User.GetUserId(), id, request, ct);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> Create(HttpContext context, INoteService noteService, CreateNoteRequest request, CancellationToken ct)
    {
        var note = await noteService.CreateAsync(context.User.GetUserId(), request, ct);
        return TypedResults.Created($"/notes/{note.Id}", note);
    }

    private static async Task<IResult> GetById(HttpContext context, INoteService noteService, Guid id, CancellationToken ct)
    {
        var note = await noteService.GetByIdAsync(context.User.GetUserId(), id, ct);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> GetAll(HttpContext context, INoteService noteService, CancellationToken ct)
    {
        var notes = await noteService.GetAllAsync(context.User.GetUserId(), ct);
        return TypedResults.Ok(notes);
    }
}
