using Sanctuary.API.Infrastructure.Constants;

namespace Sanctuary.API.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(RouteGroups.Categories);

        group.MapGet("/", GetAll);
    }

    private static async Task<IResult> GetAll(ICategoryService categoryService, CancellationToken ct)
    {
        var categories = await categoryService.GetAllAsync(ct);
        return TypedResults.Ok(categories);
    }
}
