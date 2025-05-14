using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Infrastructure.Repositories;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class DeleteAuctionCommandHandler : IRequestHandler<DeleteAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        public DeleteAuctionCommandHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository)); //*Valido que estas inyecciones sean exitosas

        }

        public async Task<Guid> Handle(DeleteAuctionCommandHandler request, CancellationToken cancellationToken)
        {
            try
            {
                var auctionId = AuctionId.Create(request.AuctionId);
                await _auctionRepository.DeleteAsync(auctionId);
                return auctionId.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}