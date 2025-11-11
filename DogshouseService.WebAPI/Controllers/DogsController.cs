using Microsoft.AspNetCore.Mvc;
using DogshouseService.BLL.Interfaces;
using DogshouseService.BLL.DTOs;

namespace DogshouseService.WebAPI.Controllers;

[ApiController]
[Route("")]
public class DogsController : ControllerBase
{
    private readonly IDogService _service;
    public DogsController(IDogService service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    public IActionResult Ping() => Content("Dogshouseservice.Version1.0.1", "text/plain");

    [HttpGet("dogs")]
    public async Task<IActionResult> GetAsync(
        [FromQuery] string? attribute,
        [FromQuery] string? order,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {

        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Page number and page size must be greater than 0.");

        var (items, total) = await _service.GetListAsync(attribute, order, pageNumber, pageSize);
        Response.Headers.Add("X-Total-Count", total.ToString());

        return Ok(items);
    }

    [HttpPost("dog")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDogRequest request)
    {
        try
        {
            var createdDog = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetAsync), new { }, createdDog);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
