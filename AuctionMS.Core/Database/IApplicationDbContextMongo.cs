﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AuctionMS.Core.Database
{
    public interface IApplicationDbContextMongo
    {
        IMongoDatabase Database { get; }


        IClientSessionHandle BeginTransaction();
    }
}

