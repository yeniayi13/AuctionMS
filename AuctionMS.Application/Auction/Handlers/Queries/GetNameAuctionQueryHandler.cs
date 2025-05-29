using MediatR;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Core.Database;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;
using AutoMapper;



namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetNameAuctionQueryHandler : IRequestHandler<GetNameAuctionQuery, GetAuctionDto>
    {
        public IAuctionRepositoryMongo _auctionRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper; // Agregar el Mapper
        public GetNameAuctionQueryHandler(IAuctionRepositoryMongo auctionRepository, IApplicationDbContext dbContext, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _dbContext = dbContext;
            _mapper = mapper;// Inyectar el Mapper
        }

        public async Task<GetAuctionDto> Handle(GetNameAuctionQuery request, CancellationToken cancellationToken)
        {
            //if (request.Id == Guid.Empty) throw new NullAttributeException("Auction id is required");
            var auctionName = AuctionName.Create(request.Name);
            var userId = AuctionUserId.Create(request.UserId);
            var productId = AuctionProductId.Create(request.UserId);
            var auction = await _auctionRepository.GetByNameAsync(auctionName!, userId!, productId!);
            var auctionDto = _mapper.Map<GetAuctionDto>(auction);
            return auctionDto;
        }
    }
}
