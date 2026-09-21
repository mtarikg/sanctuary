using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.Features.Categories.DTOs;

namespace Sanctuary.API.Features.Categories;

public class CategoryService(AppDbContext dbContext) : ICategoryService
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync(ct);
    }
}
