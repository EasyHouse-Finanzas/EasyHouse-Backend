using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyHouse.IAM.Infrastructure;

public class RecaptchaValidationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RecaptchaValidationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        var secretKey = _configuration["Recaptcha:SecretKey"];
        if (string.IsNullOrEmpty(secretKey)) throw new Exception("Recaptcha SecretKey no configurada.");
        var response = await _httpClient.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", 
            null);

        if (!response.IsSuccessStatusCode) return false;

        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GoogleRecaptchaResponse>(jsonString);

        return result?.Success ?? false;
    }
    private class GoogleRecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}