using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace SalesCRM.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocalizationController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public LocalizationController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string lang = "vi")
    {
        // Sanitize language parameter to avoid path traversal
        lang = lang.ToLower() switch
        {
            "en" => "en",
            "vi" => "vi",
            _ => "vi"
        };

        var filePath = Path.Combine(_env.ContentRootPath, "Locales", $"{lang}.json");
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { error = $"Localization file for language '{lang}' not found." });
        }

        var json = await System.IO.File.ReadAllTextAsync(filePath);
        return Content(json, "application/json");
    }
}
