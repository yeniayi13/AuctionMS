using MediatR;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetNameAuctionQuery : IRequest<GetAuctionDto>
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public GetNameAuctionQuery(string name, Guid userId, Guid productId)
        {
            Name = name;
            UserId = userId;
            ProductId = productId;
        }
    }
}
