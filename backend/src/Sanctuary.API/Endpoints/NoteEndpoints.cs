using Sanctuary.API.DTOs;
using Sanctuary.API.Services;

namespace Sanctuary.API.Endpoints;

public static class NoteEndpoints
{
    private const int MockUserId = 1;

    public static void MapNoteEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/notes");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);

    }

    private static async Task<IResult> Delete(INoteService noteService, int id)
    {
        var deleted = await noteService.DeleteAsync(MockUserId, id);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> Update(INoteService noteService, int id, UpdateNoteRequest request)
    {
        var note = await noteService.UpdateAsync(MockUserId, id, request);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> Create(INoteService noteService, CreateNoteRequest request)
    {
        var note = await noteService.CreateAsync(MockUserId, request);
        return TypedResults.Created($"/notes/{note.Id}", note);
    }

    private static async Task<IResult> GetById(INoteService noteService, int id)
    {
        var note = await noteService.GetByIdAsync(MockUserId, id);
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    private static async Task<IResult> GetAll(INoteService noteService)
    {
        var notes = await noteService.GetAllAsync(MockUserId);
        return TypedResults.Ok(notes);
    }
}
