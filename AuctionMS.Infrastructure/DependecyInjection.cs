using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Common.Primitives;
using AuctionMS.Infrastructure.Database.Context.Postgres;


namespace AuctionMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("PostgresSQLConnection");
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<IApplicationDbContext>(auction => auction.GetRequiredService<ApplicationDbContext>()!);
            services.AddScoped<IUnitOfWork>(auction => auction.GetRequiredService<ApplicationDbContext>()!);

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            

            return services;
        }
    }
}