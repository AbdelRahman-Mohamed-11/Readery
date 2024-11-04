using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

namespace Readery.Utilities.Interfaces
{

    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PayPalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetPaypalAccessToken()
        {
            string accessToken = string.Empty;
            string paypalUrl = _configuration["PayPalOptions:Url"];
            string clientId = _configuration["PayPalOptions:ClientId"];
            string secret = _configuration["PayPalOptions:ClientSecret"];
            string url = $"{paypalUrl}/v1/oauth2/token";

            var credentials64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials64);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            var httpResponse = await _httpClient.SendAsync(requestMessage);

            if (httpResponse.IsSuccessStatusCode)
            {
                var strResponse = await httpResponse.Content.ReadAsStringAsync();
                var jsonResponse = JsonNode.Parse(strResponse);
                if (jsonResponse != null)
                {
                    accessToken = jsonResponse["access_token"]?.ToString() ?? string.Empty;
                }
            }
            else
            {
                // Handle error response (log it or throw an exception)
                throw new Exception($"Unable to get access token. Status code: {httpResponse.StatusCode}");
            }

            return accessToken;
        }
    }

}
