using Microsoft.AspNetCore.Mvc;
using TorvaldsReminder.Infrastructure.Seed;

namespace TorvaldsReminder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController(DatabaseSeeder databaseSeeder) : ControllerBase
{
    [HttpPost("reset")]
    public async Task<IActionResult> ResetData()
    {
        await databaseSeeder.ClearAndSeedAsync();
        return Ok(new { message = "La data ha sido reiniciada y restaurada correctamente." });
    }
}
