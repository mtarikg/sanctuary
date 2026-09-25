using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Common;
using Sanctuary.API.Infrastructure.Constants;
using Sanctuary.API.Infrastructure.Extensions;

namespace Sanctuary.API.Features.Notes;

public static class NoteEndpoints
{
    public static void MapNoteEndPoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroups.Notes)
            .RequireAuthorization()
            .WithTags(OpenApiTags.Notes);

        group.MapGet("/", GetAll)
            .WithSummary("Get all notes for the authenticated user")
            .WithDescription("Returns a paginated list of notes. Supports page and pageSize query parameters.")
            .Produces<PagedResult<NoteResponse>>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", GetById)
            .WithSummary("Get a note by its ID")
            .Produces<NoteResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .WithSummary("Create a new note")
            .Produces<NoteResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/{id:guid}", Update)
            .WithSummary("Update an existing note")
            .Produces<NoteResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", Delete)
            .WithSummary("Delete a note")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
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

    private static async Task<IResult> GetAll(HttpContext context, INoteService noteService,
        PaginationParams pagination, CancellationToken ct)
    {
        var notes = await noteService.GetAllAsync(context.User.GetUserId(), pagination, ct);
        return TypedResults.Ok(notes);
    }
}
