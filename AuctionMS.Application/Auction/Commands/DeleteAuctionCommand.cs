using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Application.Auction.Commands
{
    public class DeleteAuctionCommand : IRequest<Guid>
    {
        public Guid AuctionId { get; set; }

        public DeleteAuctionCommand(Guid auction
            )
        {
            AuctionId = auction;
        }
    }
}
