using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetAuctionStateQuery : IRequest<string>
    {
        public Guid AuctionId { get; }

        public GetAuctionStateQuery(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

}
