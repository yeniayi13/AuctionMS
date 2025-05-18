using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Core.Service.User
{
    public interface IUserService
    {
        Task<bool> AuctioneerExists(AuctionUserId auctionUserId);

    }
}
