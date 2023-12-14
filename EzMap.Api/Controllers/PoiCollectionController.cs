using EzMap.Api.Services;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.Domain.Repositories;
using EzMap.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PoiCollectionController : ControllerBase
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> AddPoiCollection([FromBody] PoiCollectionCreateDto dto,
        [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Description))
        {
            return BadRequest("Kindly fill Name, Address fields!");
        }

        uow.PoiCollectionRepository.AddPoiCollection(dto.WithUserId(identityService.GetUserId()));

        if (await uow.SaveAsync() > 0) return Ok("Your poi collection is created successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPoiCollectionDetail(Guid id, [FromServices] IIdentityService identityService,
        [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Please provide a valid id!");
        }

        var result = await uow.PoiCollectionRepository.GetPoiCollectionById(identityService.GetUserId(), id);

        return result is not null ? Ok(result) : new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromQuery] string keyword)
    {
        var result = await uow.PoiCollectionRepository.Search(identityService.GetUserId(), keyword);
        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] PoiCollectionUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Description))
        {
            return BadRequest("Kindly fill Name, Description fields!");
        }

        var dbPoiCol = await uow.PoiCollectionRepository.GetPoiCollectionById(identityService.GetUserId(), dto.Id);

        if (dbPoiCol is not null) uow.PoiCollectionRepository.UpdatePoiCollectionAsync(dbPoiCol, dto);

        if (await uow.SaveAsync() > 0) return Ok("Your poi collection is updated successfully ");
        
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
        
        await uow.PoiCollectionRepository.DeletePoiCollectionAsync(id);
        
        if (await uow.SaveAsync() > 0) return Ok("Your poi collection is deleted successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListPoiCollection([FromServices] IUnitOfWork uow,
        [FromServices] IdentityService identityService)
    {
        var result = await uow.PoiCollectionRepository.GetListPoiCollectionAsync(identityService.GetUserId());

        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}