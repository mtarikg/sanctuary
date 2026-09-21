using Microsoft.AspNetCore.Mvc;
using Sanctuary.API.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace Sanctuary.API.Infrastructure.Common;

public record PaginationParams
{
    [FromQuery] public int Page { get; init; } = PaginationConstants.DefaultPage;
    [Range(1, 100)][FromQuery] public int PageSize { get; init; } = PaginationConstants.DefaultPageSize;
}
