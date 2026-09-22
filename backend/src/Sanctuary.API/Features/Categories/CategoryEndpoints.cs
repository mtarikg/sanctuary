using Sanctuary.API.Features.Categories.DTOs;
using Sanctuary.API.Infrastructure.Constants;

namespace Sanctuary.API.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroups.Categories)
            .WithTags(OpenApiTags.Categories);

        group.MapGet("/", GetAll)
            .WithSummary("Get all categories")
            .Produces<IEnumerable<CategoryResponse>>();
    }

    private static async Task<IResult> GetAll(ICategoryService categoryService, CancellationToken ct)
    {
        var categories = await categoryService.GetAllAsync(ct);
        return TypedResults.Ok(categories);
    }
}
