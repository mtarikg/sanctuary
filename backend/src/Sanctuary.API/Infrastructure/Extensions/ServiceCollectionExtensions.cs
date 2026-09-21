namespace Sanctuary.API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProblemDetailsConfiguration(this IServiceCollection services)
    {
        services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Title = context.ProblemDetails.Status switch
                {
                    StatusCodes.Status400BadRequest => "Invalid request.",
                    StatusCodes.Status401Unauthorized => "Authentication required.",
                    StatusCodes.Status403Forbidden => "Access denied.",
                    StatusCodes.Status404NotFound => "Resource not found.",
                    StatusCodes.Status409Conflict => "Conflict with existing resource.",
                    StatusCodes.Status429TooManyRequests => "Too many requests.",
                    StatusCodes.Status500InternalServerError => "An unexpected error occurred.",
                    _ => context.ProblemDetails.Title
                };
            };
        });

        return services;
    }
}
