using Sanctuary.API.Features.Notes.DTOs;

namespace Sanctuary.API.Features.Notes;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetAllAsync(Guid userId);
    Task<NoteResponse?> GetByIdAsync(Guid userId, Guid id);
    Task<NoteResponse> CreateAsync(Guid userId, CreateNoteRequest request);
    Task<NoteResponse?> UpdateAsync(Guid userId, Guid id, UpdateNoteRequest request);
    Task<bool> DeleteAsync(Guid userId, Guid id);
}
