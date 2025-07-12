using AutoMapper;
using MediatR;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure.Exceptions;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetAllAuctionQueryHandler : IRequestHandler<GetAllAuctionQuery, List<GetAuctionDto>>
     
    {
        public IAuctionRepositoryMongo _auctionRepository;
        private readonly IMapper _mapper; // Agregar el Mapper
       

        public GetAllAuctionQueryHandler(IAuctionRepositoryMongo auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Inyectar el Mapper
        }

        public async Task<List<GetAuctionDto>> Handle(GetAllAuctionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var auction = await _auctionRepository.GetAllAsync(AuctionUserId.Create(request.UserId));

                if (auction == null) throw new AuctionNotFoundException("Auctions are empty");
                var auctionDto = _mapper.Map<List<GetAuctionDto>>(auction);

                return auctionDto;
            }
            catch(AuctionNotFoundException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }






        }
    }
}