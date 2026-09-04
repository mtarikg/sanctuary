using Sanctuary.API.Features.Notes.DTOs;

namespace Sanctuary.API.Features.Notes;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<NoteResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<NoteResponse> CreateAsync(Guid userId, CreateNoteRequest request, CancellationToken ct = default);
    Task<NoteResponse?> UpdateAsync(Guid userId, Guid id, UpdateNoteRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default);
}
