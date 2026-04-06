using System.Text.Json;
using HappyFurnitureBE.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HappyFurnitureBE.Application.Services.Recaptcha;

public class RecaptchaService : IRecaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<RecaptchaService> _logger;

    public RecaptchaService(HttpClient httpClient, IConfiguration configuration, ILogger<RecaptchaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _secretKey = configuration["Recaptcha:SecretKey"]
            ?? throw new InvalidOperationException("Recaptcha:SecretKey is not configured.");
    }

    public async Task<bool> VerifyTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("reCAPTCHA token is empty.");
            return false;
        }

        var response = await _httpClient.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _secretKey,
                ["response"] = token
            }));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("reCAPTCHA siteverify HTTP error: {StatusCode}", response.StatusCode);
            return false;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var success = doc.RootElement.GetProperty("success").GetBoolean();

        if (!success && doc.RootElement.TryGetProperty("error-codes", out var errorCodes))
            _logger.LogWarning("reCAPTCHA failed. Error codes: {Errors}", errorCodes.ToString());

        return success;
    }
}
