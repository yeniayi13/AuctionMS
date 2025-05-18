using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AuctionMS.Common.Primitives
{
    
    public record DomainEvent(Guid id) : INotification;
}