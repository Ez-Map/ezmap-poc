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

        return await uow.SaveAsync() > 0
            ? Ok("Your point of interest is created successfully!")
            : new StatusCodeResult(StatusCodes.Status500InternalServerError);
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

        var dbPoi = await uow.PoiRepository.GetPoiById(identityService.GetUserId(), dto.Id);

        if (dbPoi is not null)
        {
            uow.PoiRepository.UpdatePoiAsync(dbPoi, dto.WithUserId(identityService.GetUserId()));
        }

        return await uow.SaveAsync() > 0
            ? Ok("Your point of interest is updated successfully!")
            : new StatusCodeResult(StatusCodes.Status500InternalServerError);
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

        return await uow.SaveAsync() > 0
            ? Ok("Your point of interest is deleted successfully!")
            : new StatusCodeResult(StatusCodes.Status500InternalServerError);
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
        [FromServices] IIdentityService identityService)
    {
        var result = await uow.PoiRepository.GetListPoiAsync(identityService.GetUserId());

        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromQuery] string keyword)
    {
        var result = await uow.PoiRepository.Search(identityService.GetUserId(), keyword);

        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}