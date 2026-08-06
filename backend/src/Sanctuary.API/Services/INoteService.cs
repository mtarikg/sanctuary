using Sanctuary.API.DTOs;

namespace Sanctuary.API.Services;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetAllAsync(int userId);
    Task<NoteResponse?> GetByIdAsync(int id, int userId);
    Task<NoteResponse> CreateAsync(int userId, CreateNoteRequest request);
    Task<NoteResponse?> UpdateAsync(int id, int userId, UpdateNoteRequest request);
    Task<bool> DeleteAsync(int id, int userId);
}
