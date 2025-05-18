using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionMS.Common.Primitives
{
    public interface IUnitOfWork
    {
       
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}