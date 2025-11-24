using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace WeddingCraft.Api.Controllers;

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

    [AllowAnonymous]
    [HttpPost("design")]
    public async Task<IActionResult> Design(DesignRequestDto dto)
    {
        var apiKey = _config["OpenAi:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var client = _http.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var prompt = $"Generate a creative wedding {dto.ProductType} design with theme '{dto.Theme}', color scheme '{dto.ColorScheme}', and details '{dto.AdditionalDetails}'.";

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = 500
        };

        var resp = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
        var json = await resp.Content.ReadAsStringAsync();

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        _db.AiRequests.Add(new AiRequest { UserId = userId, Prompt = prompt, Response = json });
        await _db.SaveChangesAsync();

        return Ok(new { response = json });
    }
}
