using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepoPgNet.Abstractions;
using RepoPgNet.Repository;

namespace RepoPgNet.Configurations;

public static class ServiceCollectionExtensions
{
    public static void AddRepoPgNet<TContext>(this IServiceCollection services, IConfiguration configuration)
    where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

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