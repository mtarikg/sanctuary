using Sanctuary.API.Features.Notes.DTOs;
using System.Security.Claims;

namespace Sanctuary.API.Features.Notes;

public static class NoteEndpoints
{
    public static void MapNoteEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/notes").RequireAuthorization();

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);

    }

    private static int GetUserId(HttpContext context) => int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static async Task<IResult> Delete(HttpContext context, INoteService noteService, int id)
    {
        var deleted = await noteService.DeleteAsync(GetUserId(context), id);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> Update(HttpContext context, INoteService noteService, int id, UpdateNoteRequest request)
    {
        var note = await noteService.UpdateAsync(GetUserId(context), id, request);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> Create(HttpContext context, INoteService noteService, CreateNoteRequest request)
    {
        var note = await noteService.CreateAsync(GetUserId(context), request);
        return TypedResults.Created($"/notes/{note.Id}", note);
    }

    private static async Task<IResult> GetById(HttpContext context, INoteService noteService, int id)
    {
        var note = await noteService.GetByIdAsync(GetUserId(context), id);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> GetAll(HttpContext context, INoteService noteService)
    {
        var notes = await noteService.GetAllAsync(GetUserId(context));
        return TypedResults.Ok(notes);
    }
}
