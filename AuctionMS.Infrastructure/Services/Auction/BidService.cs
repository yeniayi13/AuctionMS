using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Infrastructure;
using AuctionMS.Core.Service.Auction;
using AuctionMS.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using Firebase.Auth;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Infrastructure.Services.Auction
{
    [ExcludeFromCodeCoverage]
    public class BidService : IBidService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;

        public BidService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuración del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18083/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

       
        public async Task<bool> BidExists(Guid auctionBidId, Guid auctionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/bid/{auctionBidId}?auctionId={auctionId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al verificar si la puja existe en la subasta: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.Content == null || string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()))
                {
                    throw new InvalidOperationException("El contenido de la respuesta es nulo o vacío.");
                }

                return true;
            }
            catch
            {
                throw;
            }
        }



        
        public async Task<bool> IsWinningBid(Guid auctionBidId, Guid auctionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/bid/{auctionBidId}?auctionId={auctionId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al obtener la puja: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var bid = await JsonSerializer.DeserializeAsync<GetBid>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (bid == null)
                {
                    throw new InvalidOperationException("La puja no fue encontrada.");
                }

                return bid.Status.Equals("Ganadora", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                throw;
            }
        }
    }
}