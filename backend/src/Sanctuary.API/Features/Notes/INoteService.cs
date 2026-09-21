using Sanctuary.API.Features.Notes.DTOs;
using Sanctuary.API.Infrastructure.Common;

namespace Sanctuary.API.Features.Notes;

public interface INoteService
{
    Task<PagedResult<NoteResponse>> GetAllAsync(Guid userId, PaginationParams pagination, CancellationToken ct = default);
    Task<NoteResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<NoteResponse> CreateAsync(Guid userId, CreateNoteRequest request, CancellationToken ct = default);
    Task<NoteResponse?> UpdateAsync(Guid userId, Guid id, UpdateNoteRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default);
}
