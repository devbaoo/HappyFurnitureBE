using System.Text.Json;
using HappyFurnitureBE.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HappyFurnitureBE.Application.Services.Recaptcha;

public class RecaptchaService : IRecaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly double _minScore;


    public RecaptchaService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Recaptcha:SecretKey"]
            ?? throw new InvalidOperationException("Recaptcha:SecretKey is not configured.");
        _minScore = configuration.GetValue<double>("Recaptcha:MinScore", 0.5);

    }

    public async Task<bool> VerifyTokenAsync(string token)
    {
        var response = await _httpClient.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _secretKey,
                ["response"] = token
            }));

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var success = doc.RootElement.GetProperty("success").GetBoolean();
        if (!success) return false;

        // v3: kiểm tra score (0.0 - 1.0), >= 0.5 là người dùng thật
        if (doc.RootElement.TryGetProperty("score", out var scoreProp))
            return scoreProp.GetDouble() >= _minScore;

        return true;

    }
}
