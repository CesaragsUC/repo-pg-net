using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RepoPgNet;

public static class ServiceCollectionExtensions
{
    public static void AddRepoPgNet<TContext>(this IServiceCollection services, IConfiguration configuration)
    where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped(typeof(IPgRepository<>), typeof(PgRepository<>));

        //Essa linha informa ao contêiner que sempre que uma dependência de DbContext for requisitada, ele deve fornecer o ex: TContext(MeuDbContext).
        //Assim, o PgRepository <TEntity> que depende de DbContext agora pode receber MeuDbContext,
        //porque o contêiner sabe que DbContext deve ser resolvido como MeuDbContext
        services.AddScoped<DbContext, TContext>();

    }
}

//Importante:
//A classe base DbContext é genérica e não conhece MeuDbContext.
//No entanto, MeuDbContext herda de DbContext, e você precisa informar explicitamente ao contêiner que o DbContext deve ser resolvido como MeuDbContext.