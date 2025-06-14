﻿using MediatR;
using AuctionMS.Application.Auction.Queries;
using AuctionMS.Core.Repository;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionMS.Application.Auction.Handlers.Queries
{
    public class GetProductAuctionQueryHandler : IRequestHandler<GetProductAuctionQuery, GetAuctionDto>
    {
        private readonly IAuctionRepositoryMongo _auctionRepository;
        private readonly IMapper _mapper;

        public GetProductAuctionQueryHandler(IAuctionRepositoryMongo auctionRepository, IMapper mapper)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper;
        }

        public async Task<GetAuctionDto> Handle(GetProductAuctionQuery request, CancellationToken cancellationToken)
        {
            var productId = AuctionProductId.Create(request.ProductId);
            var userId = AuctionUserId.Create(request.UserId);

            var auction = await _auctionRepository.ObtenerSubastaActivaPorProductoAsync(productId,userId);

            if (auction == null)
                return null;

            return _mapper.Map<GetAuctionDto>(auction);
        }
    }
}
