using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using AuctionMS;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Infrastructure.Repositories;
using AuctionMS.Application.Auction.Handlers.Commands;
using AuctionMS.Application.Auction.Handlers.Queries;
using AuctionMS.Infrastructure.Database.Context.Postgres;
using AuctionMS.Infrastructure.Services.User;


namespace AuctionMS
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGenWithAuth(configuration);
            services.KeycloakConfiguration(configuration);

            //* Sin los Scope no funciona!!
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            //Registro de handlers 


            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateAuctionCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetNameAuctionQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAuctionQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAllAuctionQueryHandler).Assembly));
            //services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetFilteredAuctionQueryHandler).Assembly));
            services.AddHttpClient<UserService>(
                client =>
                {
                    client.BaseAddress = new Uri("https://localhost:18084");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );

            //services.AddHttpClient<ProductService>(
            //client =>
            //{
            //  client.BaseAddress = new Uri("https://localhost:18083");
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //}
            //);
            return services;
        }
    }
}
