using Sanctuary.API.Features.Categories.DTOs;

namespace Sanctuary.API.Features.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct = default);

}
