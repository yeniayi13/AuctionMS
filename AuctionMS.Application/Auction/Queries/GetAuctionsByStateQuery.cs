using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;
using MediatR;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetAuctionsByStateQuery : IRequest<List<GetAuctionDto>>
    {
        public string Estado { get; }

        public GetAuctionsByStateQuery(string estado)
        {
            Estado = estado;
        }
    }
}
