using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShoexEcommerce.Application.Interfaces;
using ShoexEcommerce.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class ShoppingAssistantService : IShoppingAssistantService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public ShoppingAssistantService(
        AppDbContext db,
        HttpClient httpClient,
        IConfiguration config)
    {
        _db = db;
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> AskAsync(int userId, string question)
    {
        var products = await _db.Products
            .Include(x => x.Brand)
            .Take(20)
            .Select(x => new
            {
                x.Name,
                x.Price,
                Brand = x.Brand.Name,
                x.Description
            })
            .ToListAsync();

        var prompt = $@"
You are Shoex AI Shopping Assistant.

Available Products:
{JsonSerializer.Serialize(products)}

Customer Question:
{question}

Answer only based on available products.
";

        return await AskGemini(prompt);
    }

    private async Task<string> AskGemini(string prompt)
    {
        var apiKey = _config["Gemini:ApiKey"];

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error (Status {response.StatusCode}): {errContent}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "No response";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AskGemini: {ex}");
            throw;
        }
    }
}