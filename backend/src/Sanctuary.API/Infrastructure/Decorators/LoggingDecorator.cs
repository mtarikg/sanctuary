namespace Sanctuary.API.Infrastructure.Decorators;

public abstract class LoggingDecorator(ILogger logger)
{
    protected ILogger Logger => logger;

    protected async Task<T?> ExecuteWithLoggingAsync<T>(Func<Task<T?>> operation, Action<T> onSuccess, Action onFailure)
    {
        var result = await operation();

        if (result is null) onFailure();
        else onSuccess(result);

        return result;
    }

    protected async Task<T> ExecuteWithLoggingAsync<T>(Func<Task<T>> operation, Action<T> onSuccess)
    {
        var result = await operation();
        onSuccess(result);

        return result;
    }
}
