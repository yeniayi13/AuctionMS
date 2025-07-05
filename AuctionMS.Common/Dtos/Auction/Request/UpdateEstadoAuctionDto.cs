using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Request
{
    public class UpdateEstadoAuctionDto
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = null!;
  
    

   
        public UpdateEstadoAuctionDto(Guid id, string currentState)
        {
            CorrelationId = id;
            CurrentState = currentState;
           

        }
    }

}