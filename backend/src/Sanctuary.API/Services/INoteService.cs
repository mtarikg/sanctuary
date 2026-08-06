using Sanctuary.API.DTOs;

namespace Sanctuary.API.Services;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetAllAsync(int userId);
    Task<NoteResponse?> GetByIdAsync(int userId, int id);
    Task<NoteResponse> CreateAsync(int userId, CreateNoteRequest request);
    Task<NoteResponse?> UpdateAsync(int userId, int id, UpdateNoteRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}
