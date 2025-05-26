using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Core.Service.Auction
{
    public interface IProductService
    {
        Task<bool> ProductExist(Guid productId);
    }
}
