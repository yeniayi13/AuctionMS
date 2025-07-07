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

        public async Task<bool> ProductExist(Guid auctionProductId, Guid auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/product/{auctionProductId}?userId={auctionUserId}");

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



        public async Task<decimal?> GetProductStock(Guid auctionProductId, Guid auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"auctioneer/product/{auctionProductId}?userId={auctionUserId}");


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

        public async Task<bool> UpdateProductStockAsync(Guid productId, decimal newStock, Guid auctionUserId)
        {
            try
            {
                // 1️⃣ Obtener los datos completos del producto
                var response = await _httpClient.GetAsync($"auctioneer/product/{productId}?userId={auctionUserId}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error obteniendo el producto: {response.StatusCode}");
                    return false;
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var productData = await JsonSerializer.DeserializeAsync<GetProduct>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (productData == null)
                {
                    Console.WriteLine("Error: No se pudo deserializar el producto.");
                    return false;
                }

                Console.WriteLine(JsonSerializer.Serialize(productData));

                // 2️⃣ Guardar el stock actual antes de modificarlo
                var currentStock = productData.ProductStock;

                // 3️⃣ Evaluar condición de agotado antes de aplicar cambios
                var updatedAvailability = newStock == 0 ? "NoDisponible" : productData.ProductAvilability;

                Console.WriteLine($"Stock actual: {currentStock}, Nuevo stock: {newStock}, Disponibilidad actualizada: {updatedAvailability}");

                productData.ProductStock = newStock;
                      Console.WriteLine($" Nuevo stock: {productData.ProductStock}, Disponibilidad actualizada: {updatedAvailability}");

                // 4️⃣ Crear el payload con la lógica aplicada
                var updatePayload = new GetProduct
                {
                    ProductId = productId,
                    ProductStock = productData.ProductStock,
                    ProductUserId = productData.ProductUserId,
                    ProductName = productData.ProductName,
                    ProductImage = productData.ProductImage,
                    ProductPrice = productData.ProductPrice != null ? Convert.ToDecimal(productData.ProductPrice) : 0,
                    ProductDescription = productData.ProductDescription,
                    ProductAvilability = updatedAvailability,
                    CategoryId = productData.CategoryId
                };

                Console.WriteLine($"User ID enviado: {productData.ProductUserId}");

                var content = new StringContent(
                    JsonSerializer.Serialize(updatePayload),
                    Encoding.UTF8,
                    "application/json"
                );
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 5️⃣ Enviar actualización
                response = await _httpClient.PutAsync($"auctioneer/product/Update-Product/{productId}?userId={auctionUserId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error actualizando el stock del producto: {response.StatusCode}");
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Detalles del error: {errorDetails}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al actualizar el stock del producto: {ex.Message}");
                throw;
            }
        }
    }
}


