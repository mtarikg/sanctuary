using Microsoft.AspNetCore.Mvc;
using Sanctuary.API.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;
namespace Sanctuary.API.Infrastructure.Common;

public class PaginationParams
{
    public int Page { get; init; }
    [Range(1, 100)] public int PageSize { get; init; }

    public static ValueTask<PaginationParams?> BindAsync(HttpContext context)
    {
        var page = ParsePositiveInt(context.Request.Query["page"]) ?? PaginationConstants.DefaultPage;
        var pageSize = ParsePositiveInt(context.Request.Query["pageSize"]) ?? PaginationConstants.DefaultPageSize;

        return ValueTask.FromResult<PaginationParams?>(new PaginationParams { Page = page, PageSize = pageSize });
    }

    private static int? ParsePositiveInt(string? value) =>
        int.TryParse(value, out var result) && result > 0 ? result : null;
}
