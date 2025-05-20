using Microsoft.Extensions.DependencyInjection;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Infrastructure.Database.Context.Postgres;

namespace AuctionMS
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGenWithAuth(configuration);
            //services.KeycloakConfiguration(configuration);

            //* Sin los Scope no funciona!!
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();

            //Registro de handlers 
          
           
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetNameAuctionQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAuctionQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAllAuctionQueryHandler).Assembly));
            //services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetFilteredAuctionQueryHandler).Assembly));
            return services;
        }
    }
}
