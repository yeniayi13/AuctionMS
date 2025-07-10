using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionMS.Core.Service.Auction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Core.Repository;

namespace AuctionMS.Infrastructure.Services.Auction
{
    [ExcludeFromCodeCoverage]
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionRepositoryMongo _auctionRepositoryMongo;



        public PaymentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;
            IAuctionRepository auctionRepository;
            IAuctionRepositoryMongo auctionRepositoryMongo;

            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18088/"); // Cambia al host real del microservicio de pagos
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<string?> GetPaymentIdByAuctionIdAsync(string auctionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"payments/by-auction/{auctionId}");

                if (!response.IsSuccessStatusCode)
                {
                    return null; 
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var paymentInfo = await JsonSerializer.DeserializeAsync<PaymentResponse>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paymentInfo?.PaymentId;
            }
            catch
            {
                throw;
            }
        }

      /*  public async Task<bool> CompleteAuctionIfPaidAsync(string auctionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"payments/by-auction/{auctionId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[PAYMENT] No hay pago confirmado para la subasta {auctionId}.");
                    return false;
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var paymentInfo = await JsonSerializer.DeserializeAsync<PaymentResponse>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (string.IsNullOrEmpty(paymentInfo?.PaymentId))
                {
                    Console.WriteLine($"[PAYMENT] La respuesta no contiene PaymentId válido.");
                    return false;
                }

                var auction = await _auctionRepositoryMongo.GetByIdAsync(auctionId);
                if (auction == null)
                {
                    Console.WriteLine($"[AUCTION] La subasta con ID {auctionId} no existe.");
                    return false;
                }

                auction.AuctionEstado = AuctionEstado.Create("Completed");

                await _auctionRepository.UpdateAsync(auction);

                Console.WriteLine($"[AUCTION] Subasta {auctionId} marcada como 'Completed' por pago confirmado.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al completar subasta {auctionId}: {ex.Message}");
                throw;
            }
        }*/


    }

    public class PaymentResponse
    {
        public string? PaymentId { get; set; }
    }

    }
