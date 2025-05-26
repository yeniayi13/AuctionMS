using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuctionMS.Domain.Entities.Auction;


namespace AuctionMS.Core.Database
{
    public interface IApplicationDbContext
    {
        DbContext DbContext { get; }
        DbSet<AuctionEntity> Auction { get; set; }


        IDbContextTransactionProxy BeginTransaction();

        void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default);
    }
}