using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Readery.Core.Models.Paypal; // Corrected namespace

public class PayPalClient
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _mode;
    private readonly HttpClient _httpClient;
    private string _accessToken;
    private DateTime _tokenExpiry;

    public PayPalClient(IConfiguration configuration, HttpClient httpClient)
    {
        _clientId = configuration["PayPalOptions:ClientId"];
        _clientSecret = configuration["PayPalOptions:ClientSecret"];
        _mode = configuration["PayPalOptions:Mode"];
        _httpClient = httpClient;
    }

    private string GetBaseUrl()
    {
        return _mode.Equals("Live", StringComparison.OrdinalIgnoreCase)
            ? "https://api.paypal.com"
            : "https://api-m.sandbox.paypal.com";
    }

    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken;
        }

        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, $"{GetBaseUrl()}/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        dynamic obj = JsonConvert.DeserializeObject(json);
        _accessToken = obj.access_token;
        int expiresIn = obj.expires_in;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Buffer time

        // Optionally, store the access token in session
        // HttpContext.Session.SetString("PayPalAccessToken", _accessToken);

        return _accessToken;
    }

    public async Task<HttpClient> GetHttpClientAsync()
    {
        var token = await GetAccessTokenAsync();
        _httpClient.BaseAddress = new Uri(GetBaseUrl());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return _httpClient;
    }
}
