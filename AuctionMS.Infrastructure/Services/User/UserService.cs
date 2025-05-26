using Microsoft.Extensions.Options;
using AuctionMS.Core.Service.User;
using AuctionMS.Infrastructure;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using Microsoft.AspNetCore.Http;
using AuctionMS.Infrastructure;
using AuctionMS.Common.Dtos.Auction.Response;
using System.Text.Json;

namespace AuctionMS.Infrastructure.Services.User
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _httpClientUrl;
        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<HttpClientUrl> httpClientUrl)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClientUrl = httpClientUrl.Value.ApiUrl;

            //* Configuracion del HttpClient
            var headerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            _httpClient.BaseAddress = new Uri("http://localhost:18084/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<GetUser> AuctioneerExists(Guid auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/auctioneer/{auctionUserId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error al obtener usuario: {response.StatusCode}");
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();

                if (responseStream == null)
                {
                    throw new InvalidOperationException("El contenido de la respuesta es nulo.");
                }

                // 🔹 Asegurar que `responseStream` contiene datos antes de deserializar
                var user = await JsonSerializer.DeserializeAsync<GetUser>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (user == null)
                {
                    throw new InvalidOperationException("No se pudo deserializar el usuario.");
                }

                Console.WriteLine($"User ID: {user.UserId}, Name: {user.UserName}");

                return user;
            }
            catch
            {
                throw;
            }
        }
    }
}
