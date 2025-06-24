using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Common.Dtos.Auction.Request;
using MediatR;

namespace AuctionMS.Application.Auction.Commands
{
    
    
        public class UpdateEstadoAuctionCommand : IRequest<Guid>
    {
            public UpdateEstadoAuctionCommand(UpdateEstadoAuctionDto estadoDto)
            {
                EstadoDto = estadoDto;
            }

            public UpdateEstadoAuctionDto EstadoDto { get; private set; }
        }
 }

