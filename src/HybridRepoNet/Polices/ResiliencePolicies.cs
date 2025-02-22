using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;

namespace HybridRepoNet.Polices;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"[Polly] HTTP Retry {retryCount} - Waiting {timespan.TotalSeconds} sec due to: {outcome.Exception?.Message}");
                });
    }

    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, timespan) =>
                {
                    Console.WriteLine($"[Polly] Circuit Breaker Open - Blocking calls for {timespan.TotalSeconds} sec");
                },
                onReset: () => Console.WriteLine("[Polly] Circuit Breaker Reset - Allowing calls again"));
    }

    public static IAsyncPolicy GetDatabaseRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[Polly] Database Retry {retryCount} - Waiting {timeSpan.TotalSeconds} sec due to: {exception.Message}");
                });
    }
}