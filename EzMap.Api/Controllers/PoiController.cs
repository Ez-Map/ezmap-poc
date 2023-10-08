using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class PoiController : ControllerBase
{

    [HttpPost("CreatePoi")]
    public async Task<IActionResult> Create([FromBody] PoiCreateDto dto,
        [FromServices] IPoiRepository poiRepository)
    {
            if (string.IsNullOrEmpty(dto.Name)
                && string.IsNullOrEmpty(dto.Address))
            {
                return BadRequest("Kindly fill Name, Address fields!");
            }

            if (await poiRepository.AddPoi(dto)) return Ok("Your point of interest is created successfully!");

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
    
}