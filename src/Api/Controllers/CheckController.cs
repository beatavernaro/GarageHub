using GarageHub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckController(VehicleRepository vehicleRepository) : ControllerBase
{
    private readonly VehicleRepository _vehicleRepository = vehicleRepository;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API is running.");
    }

    [HttpGet("database")]
    public async Task<IActionResult> TestDatabase()
    {
        var result = await _vehicleRepository.TestConnectionAsync();

        return Ok(new
        {
            databaseConnected = result
        });
    }
}