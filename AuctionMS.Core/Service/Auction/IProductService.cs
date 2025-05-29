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
<<<<<<< HEAD
        Task<bool> ProductExist(Guid productId);
        Task<decimal?> GetProductStock(Guid auctionProductId);

=======
        Task<bool> ProductExist(Guid productId,Guid userId);
        Task<decimal?> GetProductStock(Guid auctionProductId, Guid auctionUserId);
>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)
    }
}
