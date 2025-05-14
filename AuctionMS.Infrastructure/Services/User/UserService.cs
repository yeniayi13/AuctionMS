using Microsoft.Extensions.Options;
using AuctionMS.Core.Service.User;
using AuctionMS.Infrastructure;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using Microsoft.AspNetCore.Http;
using AuctionMS.Infrastructure;

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
            _httpClient.BaseAddress = new Uri(_httpClientUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {headerToken}");
        }

        public async Task<bool> AuctioneerExists(AuctionUserId auctionUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/driver/{auctionUserId.Value}");
                if (!response.IsSuccessStatusCode)
                {
                    //throw new NotFoundException("Driver not found");
                }
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
