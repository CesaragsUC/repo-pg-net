using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;

namespace HybridRepoNet.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddRepoPgNet<TContext>(this IServiceCollection services, IConfiguration configuration, string provider)
    where TContext : DbContext
    {
        if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));
        }
        else if (provider.Equals("SQLServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
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

        //This line tells the container that whenever a dependency on DbContext is requested, it should provide the following: TContext(MyDbContext).
        //So, the PgRepository <TEntity> that depends on DbContext can now receive MyDbContext,
        //because the container knows that DbContext should be resolved as MyDbContext
        services.AddScoped<DbContext, TContext>();

    }
}