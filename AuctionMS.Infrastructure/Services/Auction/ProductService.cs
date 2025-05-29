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
<<<<<<< HEAD
using System.Net.Http;
=======
using Firebase.Auth;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)

namespace AuctionMS.Infrastructure.Services.Auction
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;
        public ProductService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuracion del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18083/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<bool> ProductExist(Guid auctionProductId,Guid auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/product/{auctionProductId}?userId={ auctionUserId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al verificar si ese producto se encuentra en una subasta: {response.StatusCode}");
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
<<<<<<< HEAD
        public async Task<decimal?> GetProductStock(Guid auctionProductId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auction/product/{auctionProductId}");
=======
        public async Task<decimal?> GetProductStock(Guid auctionProductId, Guid auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/product/{auctionProductId}?userId={auctionUserId}");
>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al obtener el stock del producto: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var product = await JsonSerializer.DeserializeAsync<GetProduct>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (product == null)
                {
                    throw new InvalidOperationException("El producto no fue encontrado.");
                }

                return product.ProductStock;
            }
            catch
            {
                throw;
            }
        }
<<<<<<< HEAD

=======
>>>>>>> d363556 (FIX se arreglo las peticione HTTP para product y user)
    }
}


