using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using AuctionMS.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AuctionMS.Infrastructure.Database.Context.Postgres;
using AuctionMS.Infrastructure.Database.Factory.Postgres;
using AuctionMS.Infrastructure.Database.Context.Postgres;
using AuctionMS.Infrastructure.Database.Factory.Postgres;

namespace AuctionMS.Test.Infrastructure.DataBase
{
    public class ApplicationDbContextFactoryTests
    {
        [Fact]
        public void CreateDbContext_ShouldReturnValidDbContext()
        {
            // Arrange
            var factory = new ApplicationDbContextFactory();

            // Act
            var dbContext = factory.CreateDbContext(new string[] { });

            // Assert
            Assert.NotNull(dbContext);
            Assert.IsType<ApplicationDbContext>(dbContext);
        }

        [Fact]
        public void CreateDbContext_ShouldUsePostgreSQLConfiguration()
        {
            // Arrange
            var factory = new ApplicationDbContextFactory();

            // Act
            var dbContext = factory.CreateDbContext(new string[] { });

            // Assert
            Assert.Contains("Npgsql", dbContext.Database.ProviderName); // 🔥 Confirma que está usando PostgreSQL
        }
    }

}
