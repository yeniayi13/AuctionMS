using AuctionMS.Application.Auction.Commands;
using AuctionMS.Application.Saga.Events;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Core.Repository;
using AuctionMS.Core.Service.Auction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AuctionMS.Application.Auction.ServiceBack
{
    public class AuctionActivatorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeZoneInfo _caracasZone;

        public AuctionActivatorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _caracasZone = TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[AUCTION-CYCLE] Servicio activación/cierre iniciado ");

            while (!stoppingToken.IsCancellationRequested)
            {
                var ahoraUtc = DateTime.UtcNow;
                var ahoraCaracas = TimeZoneInfo.ConvertTimeFromUtc(ahoraUtc, _caracasZone);
                Console.WriteLine($"⏱️ {ahoraUtc} UTC | 🕒 {ahoraCaracas} Caracas");

                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IAuctionRepositoryMongo>();
                var sender = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // 🟢 Activar subastas
               var pendientes = await repository.GetAllByEstadoAsync("Pending");

                if (pendientes is { Count: > 0 })
                {
                    Console.WriteLine($"[ACTIVATE] Subastas pendientes encontradas: {pendientes.Count}");

                    foreach (var auction in pendientes)
                    {
                        if (auction.AuctionFechaInicio.Value <= ahoraCaracas)
                        {
                            var updateDto = new UpdateAuctionDto
                            {
                               AuctionID = auction.AuctionId.Value,
                                AuctionName = auction.AuctionName.Value,
                                AuctionImage = auction.AuctionImage.Url,
                                AuctionPriceBase = auction.AuctionPriceBase.Value,
                                AuctionPriceReserva = auction.AuctionPriceReserva.Value,
                                AuctionDescription = auction.AuctionDescription.Value,
                                AuctionIncremento = auction.AuctionIncremento.Value,
                                AuctionCantidadProducto = auction.AuctionCantidadProducto.Value,
                                AuctionEstado = "Active",
                                AuctionFechaInicio = auction.AuctionFechaInicio.Value,
                                AuctionFechaFin = auction.AuctionFechaFin.Value,
                                AuctionCondiciones = auction.AuctionCondiciones.Value,
                                AuctionUserId = auction.AuctionUserId.Value,
                                AuctionProductId = auction.AuctionProductId.Value,
                               // AuctionBidId = auction.AuctionBidId.Value
                            };

                            await mediator.Send(new UpdateAuctionCommand
                            (
                                auction.AuctionId.Value,
                                updateDto,
                                auction.AuctionUserId.Value

                            ));

                            var endpoint = await sender.GetSendEndpoint(new Uri("queue:EstadoAuction"));
                            await endpoint.Send(new ActivateAuctionEvent
                            {
                                AuctionId = auction.AuctionId.Value,
                                FechaActivacion = ahoraCaracas
                            });

                            Console.WriteLine($"[ACTIVATE] ✅ Auction {auction.AuctionId} activada y marcada como Active");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[ACTIVATE] ❌ No hay subastas pendientes");
                }

                //  Cancelar subastas pendientes 
             /*   var pendientesCancelables = await repository.GetAllByEstadoAsync("Pending");

                if (pendientesCancelables is { Count: > 0 })
                {
                    Console.WriteLine($"[CANCEL] Subastas pendientes a evaluar para cancelación: {pendientesCancelables.Count}");

                    foreach (var auction in pendientesCancelables)
                    {
                        if (auction.AuctionFechaInicio.Value.AddMinutes(10) < ahoraCaracas)
                        {
                            try
                            {
                                await mediator.Send(new CancelAuctionCommand(
                                    auction.AuctionId.Value,
                                    auction.AuctionUserId.Value
                                ));

                                Console.WriteLine($"[CANCEL] Auction {auction.AuctionId.Value} cancelada por fecha de inicio vencida");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[CANCEL] Error al cancelar Auction {auction.AuctionId.Value}: {ex.Message}");
                            }

                            await Task.Delay(100); 
                        }
                    }
                }*/

                //Pago recibido, subasta completed

                // 🔴 Finalizar subastas activas
                var activas = await repository.GetAllByEstadoAsync("Active");

                if (activas is { Count: > 0 })
                {
                    Console.WriteLine($"[FINISH] Subastas activas encontradas: {activas.Count}");

                    foreach (var auction in activas)
                    {
                        if ( auction.AuctionFechaFin.Value <= ahoraCaracas)
                        {
                            

                            var endpoint = await sender.GetSendEndpoint(new Uri("queue:EstadoAuction"));
                            await endpoint.Send(new AuctionEndedEvent(
                                auction.AuctionId.Value,
                                auction.AuctionFechaFin.Value
                            ));

                            var updateDto = new UpdateAuctionDto
                            {
                                AuctionID = auction.AuctionId.Value,
                                AuctionName = auction.AuctionName.Value,
                                AuctionImage = auction.AuctionImage.Url,
                                AuctionPriceBase = auction.AuctionPriceBase.Value,
                                AuctionPriceReserva = auction.AuctionPriceReserva.Value,
                                AuctionDescription = auction.AuctionDescription.Value,
                                AuctionIncremento = auction.AuctionIncremento.Value,
                                AuctionCantidadProducto = auction.AuctionCantidadProducto.Value,
                                AuctionEstado = "Ended",
                                AuctionFechaInicio = auction.AuctionFechaInicio.Value,
                                AuctionFechaFin = auction.AuctionFechaFin.Value,
                                AuctionCondiciones = auction.AuctionCondiciones.Value,
                                AuctionUserId = auction.AuctionUserId.Value,
                                AuctionProductId = auction.AuctionProductId.Value
                              //  AuctionBidId = auction.AuctionBidId.Value



                            };

                            await mediator.Send(new UpdateAuctionCommand
                            (
                                auction.AuctionId.Value,
                                updateDto,
                                auction.AuctionUserId.Value

                            ));

                            Console.WriteLine($"[FINISH]  Auction {auction.AuctionId} finalizada y marcada como Ended");
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[FINISH]  No hay subastas activas para finalizar");
                }

                Console.WriteLine("[AUCTION-CYCLE]  Esperando siguiente ciclo...");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);




                // 🔵 Marcar subastas como Completed si se ha recibido el pago
             /*   var finalizadas = await repository.GetAllByEstadoAsync("Ended");

                if (finalizadas is { Count: > 0 })
                {
                    Console.WriteLine($"[COMPLETE] Subastas finalizadas encontradas: {finalizadas.Count}");

                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                    foreach (var auction in finalizadas)
                    {
                        var paymentId = await paymentService.GetPaymentIdByAuctionIdAsync(auction.AuctionId.Value.ToString());

                        if (!string.IsNullOrEmpty(paymentId))
                        {
                            var endpoint = await sender.GetSendEndpoint(new Uri("queue:EstadoAuction"));
                            await endpoint.Send(new PaymentReceivedEvent(
                                auction.AuctionId.Value,
                                ahoraCaracas // Fecha en que se detecta el pago
                            ));

                            var updateDto = new UpdateAuctionDto
                            {
                                AuctionID = auction.AuctionId.Value,
                                AuctionName = auction.AuctionName.Value,
                                AuctionImage = auction.AuctionImage.Url,
                                AuctionPriceBase = auction.AuctionPriceBase.Value,
                                AuctionPriceReserva = auction.AuctionPriceReserva.Value,
                                AuctionDescription = auction.AuctionDescription.Value,
                                AuctionIncremento = auction.AuctionIncremento.Value,
                                AuctionCantidadProducto = auction.AuctionCantidadProducto.Value,
                                AuctionEstado = "Completed",
                                AuctionFechaInicio = auction.AuctionFechaInicio.Value,
                                AuctionFechaFin = auction.AuctionFechaFin.Value,
                                AuctionCondiciones = auction.AuctionCondiciones.Value,
                                AuctionUserId = auction.AuctionUserId.Value,
                                AuctionProductId = auction.AuctionProductId.Value,
                                AuctionBidId = auction.AuctionBidId.Value,
                                AuctionPaymentId = auction.AuctionPaymentId.Value
                            };

                            await mediator.Send(new UpdateAuctionCommand
                            (
                                auction.AuctionId.Value,
                                updateDto,
                                auction.AuctionUserId.Value
                            ));

                            Console.WriteLine($"[COMPLETE] ✅ Subasta {auction.AuctionId} marcada como Completed por pago recibido.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[COMPLETE] ❌ No hay subastas finalizadas con pago aún.");
                }*/
            }

           




            Console.WriteLine("[AUCTION-CYCLE]  Servicio detenido");
        }
    }
}
