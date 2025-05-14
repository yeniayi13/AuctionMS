﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionMS.Core.Database
{
    public interface IDbContextTransactionProxy : IDisposable
    {

        void Commit();
        void Rollback();
    }
}