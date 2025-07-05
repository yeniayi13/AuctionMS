using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace AuctionMS.Domain.Entities.Auction
{

        public class EstadoAuction : SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public string CurrentState { get; set; }

            public Guid AuctionId { get; set; }

            public DateTime FechaInicio { get; set; }

            public EstadoAuction(Guid correlationId, string currentState, Guid auctionId, DateTime fechaInicio)
            {
                CorrelationId = correlationId;
                CurrentState = currentState;
                AuctionId = auctionId;
                FechaInicio = fechaInicio;

            }

        }

    }







