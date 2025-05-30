
using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using AuctionMS.Application;
using Xunit;

namespace ProductsMS.Test.Application
{
    
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddApplication_ShouldRegisterMediatR()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplication();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mediatRService = serviceProvider.GetService<IMediator>();
            Assert.NotNull(mediatRService);
        }
    }
}
