namespace Sanctuary.API.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/categories");

        group.MapGet("/", GetAll);
    }

    private static async Task<IResult> GetAll(ICategoryService categoryService)
    {
        var categories = await categoryService.GetAllAsync();
        return TypedResults.Ok(categories);
    }
}
