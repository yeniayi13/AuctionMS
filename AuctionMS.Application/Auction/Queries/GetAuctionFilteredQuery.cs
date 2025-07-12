using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;
using MediatR;

namespace AuctionMS.Application.Auction.Queries
{
    public class GetAuctionFilteredQuery : IRequest<List<GetAuctionDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public GetAuctionFilteredQuery(DateTime? startDate = null, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
