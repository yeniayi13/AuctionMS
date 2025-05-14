using MediatR;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Common.Exceptions;
using AuctionMS.Application.Auctions.Queries;
using AuctionMS.Infrastructure.Repositories;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionQueryHandler : IRequestHandler<GetAuctionQuery, GetAuctionDto>
    {
        public IAuctionRepository _auctionRepository;
        private readonly IApplicationDbContext _dbContext;

        public GetProductQueryHandler(IAuctionRepository auctionRepository, IApplicationDbContext dbContext)
        {
            _auctionRepository = auctionRepository;
            _dbContext = dbContext;
        }

        public async Task<GetAuctionDto> Handle(GetAuctionQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) throw new NullAttributeException("Auction id is required");
            var auctionId = auctionId.Create(request.Id);
            var auction = await _auctionRepository.GetByIdAsync(auctionId!);
            var createdBy = auction.CreatedBy ?? string.Empty;

            return new GetAuctionDto(
               auction.Id.Value,
                    auction.Name.Value,
                    auction.Image.Url,
                    auction.PriceBase.Value,
                    auction.PriceReserva.Value,
                    auction.Description.Value,
                    auction.Incremento.Value,
                    auction.Duracion.Value,
                    auction.Condiciones.Value,
                createdBy
            );
        }
    }
}