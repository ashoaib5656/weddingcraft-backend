using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _config;

    public AiController(ApplicationDbContext db, IHttpClientFactory http, IConfiguration config)
    {
        _db = db; _http = http; _config = config;
    }

    [HttpPost("design")]
    public async Task<IActionResult> Design(DesignRequestDto dto)
    {
        var apiKey = _config["Gemini:ApiKey"];
        var client = _http.CreateClient();

        var prompt = $"Generate a creative wedding {dto.ProductType} design with theme '{dto.Theme}', color scheme '{dto.ColorScheme}', and details '{dto.AdditionalDetails}'.";

        var body = new
        {
            contents = new[]
            {
            new {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
        };

        string model = "gemini-1.5-flash";

        var resp = await client.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}",
            body
        );

        var json = await resp.Content.ReadAsStringAsync();

        // Parse JSON
        dynamic? parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        // SAFE null-handling
        string aiText = parsed?.candidates?[0]?.content?.parts?[0]?.text ?? "No response";

        // Save to DB if user logged in
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            var userId = Guid.Parse(claim.Value);
            _db.AiRequests.Add(new AiRequest { UserId = userId, Prompt = prompt, Response = aiText });
            await _db.SaveChangesAsync();
        }

        return Ok(new { response = aiText });
    }

    [AllowAnonymous]
    [HttpGet("models")]
    public async Task<IActionResult> GetModels()
    {
        var apiKey = _config["Gemini:ApiKey"];
        var client = _http.CreateClient();

        var resp = await client.GetAsync(
            $"https://generativelanguage.googleapis.com/v1/models?key={apiKey}"
        );

        var json = await resp.Content.ReadAsStringAsync();
        return Ok(json);
    }

}
