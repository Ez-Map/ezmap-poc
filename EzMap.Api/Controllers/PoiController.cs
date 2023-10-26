using System.Security.Claims;
using EzMap.Api.Services;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using EzMap.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PoiController : ControllerBase
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] PoiCreateDto dto, [FromServices] IUnitOfWork uow, 
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Address))
        {
            return BadRequest("Kindly fill Name, Address fields!");
        }

        uow.PoiRepository.AddPoi(dto.WithUserId(identityService.GetUserId()));

        if (await uow.SaveAsync() > 0) return Ok("Your point of interest is created successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }


    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] PoiUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Address))
        {
            return BadRequest("Kindly fill Name, Address fields!");
        }

        await uow.PoiRepository.UpdatePoiAsync(dto.WithUserId(identityService.GetUserId()));

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

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPoiDetails(Guid id, [FromServices] IIdentityService identityService,
        [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Please provide a valid id!");
        }

        var result = await uow.PoiRepository.GetPoiById(identityService.GetUserId(), id);

        return result is not null ? Ok(result) : new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListPoi([FromServices] IUnitOfWork uow,
        [FromServices] IdentityService identityService)
    {

        var result = await uow.PoiRepository.GetListPoiAsync(identityService.GetUserId());

        return result.Count > 0 ? Ok(result) : Ok("Currently, there is no poi!");
    }
}