using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using AuctionMS.Infrastructure.Database.Context.Postgres;

namespace AuctionMS.Infrastructure.Database.Factory.Postgres
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=AuctionMS;Username=postgres;Password=17092002-");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
