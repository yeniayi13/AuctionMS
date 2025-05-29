using AutoMapper;
using MediatR;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Application.Auctions.Queries;


namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionQueryHandler : IRequestHandler<GetAuctionQuery, GetAuctionDto>
    {
        public IAuctionRepositoryMongo _auctionRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAuctionQueryHandler(IAuctionRepositoryMongo auctionRepository, IApplicationDbContext dbContext, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _dbContext = dbContext;
            _mapper = mapper; // Inyectar el Mapper
        }

        public async Task<GetAuctionDto> Handle(GetAuctionQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty) throw new NullAttributeException("Auction id is required");
            var auctionId = AuctionId.Create(request.Id);
            var auction = await _auctionRepository.GetByIdAsync(auctionId!, AuctionUserId.Create(request.UserId), AuctionProductId.Create(request.ProductId));
            var auctionDto = _mapper.Map<GetAuctionDto>(auction);
            return auctionDto;
        }
    }
}
