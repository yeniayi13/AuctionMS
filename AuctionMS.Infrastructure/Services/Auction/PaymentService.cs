using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionMS.Core.Service.Auction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Infrastructure.Services.Auction
{
    [ExcludeFromCodeCoverage]
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;

        public PaymentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18084/"); // Cambia al host real del microservicio de pagos
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<string?> GetPaymentIdByAuctionIdAsync(string auctionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"payments/by-auction/{auctionId}");

                if (!response.IsSuccessStatusCode)
                {
                    return null; // No hay pago registrado
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
    }

    public class PaymentResponse
    {
        public string? PaymentId { get; set; }
    }
}
