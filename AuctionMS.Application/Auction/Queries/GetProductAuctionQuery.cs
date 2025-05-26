using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;
using MediatR;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetProductAuctionQuery : IRequest<GetAuctionDto>
    {
        public Guid ProductId { get; }
        public Guid UserId { get; }

        public GetProductAuctionQuery(Guid productId, Guid userId)
        {
            ProductId = productId;
            UserId = userId;
        }
    }
}


