using Microsoft.AspNetCore.Mvc;
using TorvaldsReminder.Application.Interfaces;

namespace TorvaldsReminder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await repo.GetAllAsync());
}
