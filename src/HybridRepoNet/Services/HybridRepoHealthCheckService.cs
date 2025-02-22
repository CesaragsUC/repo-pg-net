using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Serilog;

namespace HybridRepoNet.Services;

public class HybridRepoHealthCheckService<TContext> : BackgroundService
    where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HybridRepoHealthCheckService<TContext>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

    public HybridRepoHealthCheckService(IServiceProvider serviceProvider, ILogger<HybridRepoHealthCheckService<TContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<DbUpdateException>()
            .Or<TimeoutException>()
            .Or<Npgsql.NpgsqlException>()
            .Or<Microsoft.Data.SqlClient.SqlException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Backoff exponencial
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Log.Error("[Polly] HybridRepoNet Retry {RetryCount} - Waiting {TimeSpan} sec due to: {ExceptionMessage}", retryCount, timeSpan.TotalSeconds, exception.Message);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckDatabaseConnectionAsync();
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckDatabaseConnectionAsync()
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

                Log.Information("[HybridRepoHealthCheck] Testing database connection...");

                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");

                _logger.LogInformation("[HybridRepoHealthCheck] Database is reachable.");
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex,"[HybridRepoHealthCheck] Database connection failed: {Message}", ex.Message);
        }
    }
}