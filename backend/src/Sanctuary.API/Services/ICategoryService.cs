using Sanctuary.API.DTOs;

namespace Sanctuary.API.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

}
