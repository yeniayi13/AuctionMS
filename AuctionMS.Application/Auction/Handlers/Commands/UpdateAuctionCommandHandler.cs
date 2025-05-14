using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auction.Commands;
using AuctionMS.Common.Enum;
using AuctionMS.Common.Exceptions;
using AuctionMS.Infrastructure.Repositories;

namespace AuctionMS.Application.Auction.Handlers.Commands
{
    public class UpdateAuctionCommandHandler : IRequestHandler<UpdateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        public UpdateAuctionCommandHandler(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository)); //*Valido que estas inyecciones sean exitosas


        }

        public async Task<Guid> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken)
        {
        }

    }

}