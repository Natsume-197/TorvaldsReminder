using Microsoft.AspNetCore.Mvc;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Application.Services;

namespace TorvaldsReminder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController(IInvoiceRepository repo, InvoiceProcessingService processor) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await repo.GetAllAsync());

    [HttpPost("process")]
    public async Task<IActionResult> Process()
    {
        await processor.ProcessAsync();
        return Ok(new { message = "Proceso completado" });
    }
}
