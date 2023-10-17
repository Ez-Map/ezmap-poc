using System.Security.Claims;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PoiController : ControllerBase
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] PoiCreateDto dto,
        [FromServices] IHttpContextAccessor httpContextAccessor, [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Address))
        {
            return BadRequest("Kindly fill Name, Address fields!");
        }

        var succssful = Guid.TryParse(httpContextAccessor?.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);

        var userId = succssful ? (Guid?)tempId : null; //TODO: fix this

        uow.PoiRepository.AddPoi(dto.WithUserId(userId ?? Guid.NewGuid()));

        if (await uow.SaveAsync() > 0) return Ok("Your point of interest is created successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }


    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] PoiUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Address))
        {
            return BadRequest("Kindly fill Name, Address fields!");
        }
        var succssful = Guid.TryParse(httpContextAccessor?.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);

        var userId = succssful ? (Guid?)tempId : null; //TODO: fix this

        await uow.PoiRepository.UpdatePoiAsync(dto.WithUserId(userId ?? Guid.Empty));

        if (await uow.SaveAsync() > 0) return Ok("Your point of interest is updated successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Please provide a valid id!");
        }

        await uow.PoiRepository.DeletePoiAsync(id);

        if (await uow.SaveAsync() > 0) return Ok("Your point of interest is deleted successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}