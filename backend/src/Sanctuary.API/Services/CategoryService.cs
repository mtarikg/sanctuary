using Microsoft.EntityFrameworkCore;
using Sanctuary.API.Data;
using Sanctuary.API.DTOs;

namespace Sanctuary.API.Services;

public class CategoryService(AppDbContext dbContext) : ICategoryService
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        return await dbContext.Categories
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync();
    }
}
