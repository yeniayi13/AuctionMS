using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Repository;
using AutoMapper;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetBidAuctionQueryHandler : IRequestHandler<GetBidAuctionQuery, GetAuctionDto>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository; 
        private readonly IMapper _mapper;

        public GetBidAuctionQueryHandler(IAuctionRepositoryMongo auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper;
        }

        public async Task<GetAuctionDto> Handle(GetBidAuctionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var auctionBidId = AuctionBidId.Create(request.AuctionBidId);
                var bid = await _auctionRepository.GetBidByIdAndAuctionIdAsync(auctionBidId);
                if (bid == null) return null;

                return _mapper.Map<GetAuctionDto>(bid);
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }
    }
}
