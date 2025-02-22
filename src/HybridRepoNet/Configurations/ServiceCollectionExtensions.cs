using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridRepoNet.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddHybridRepoNet<TContext>(this IServiceCollection services, IConfiguration configuration, string provider)
    where TContext : DbContext
    {
        if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"), npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5)));
        }
        else if (provider.Equals("SQLServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure(5)));
        }
        else
        {
            throw new ArgumentException($"Provider {provider} not supported");
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        services
        .AddTransient<IMediator, Mediator>()
        .AddTransient<IDomainEvent, DomainEvent>();

        services.AddScoped<DbContext, TContext>();

    }
}