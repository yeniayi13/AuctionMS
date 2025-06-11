using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Infrastructure.Exceptions;
using AutoMapper;
using MediatR;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAuctionByIdQueryHandler : IRequestHandler<GetAuctionByIdQuery, GetAuctionDto>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAuctionByIdQueryHandler(
            IAuctionRepositoryMongo auctionRepository,
            IApplicationDbContext dbContext,
            IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetAuctionDto> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                throw new NullAttributeException("Auction ID is required.");
            }

            var auctionId = AuctionId.Create(request.Id);
            var auction = await _auctionRepository.GetByIdAsync(auctionId!);

            if (auction == null)
            {
                throw new AuctionNotFoundException("Auction not found.");
            }

            return _mapper.Map<GetAuctionDto>(auction);
        }
    }
}