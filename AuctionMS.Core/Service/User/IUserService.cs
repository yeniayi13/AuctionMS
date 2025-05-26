using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;


namespace AuctionMS.Core.Service.User
{
    public interface IUserService
    {
        Task<GetUser> AuctioneerExists(Guid auctionUserId);

    }
}
