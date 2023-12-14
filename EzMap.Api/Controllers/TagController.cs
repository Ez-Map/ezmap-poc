using EzMap.Api.Services;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using EzMap.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] TagCreateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Description))
        {
            return BadRequest("Kindly fill Name, Description fields!");
        }
        
        uow.TagRepository.AddTag(dto.WithUserId(identityService.GetUserId()));

        if (await uow.SaveAsync() > 0)
        {
            return Ok("Your tag is created successfully!");
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        if (string.IsNullOrEmpty(dto.Name)
            && string.IsNullOrEmpty(dto.Description))
        {
            return BadRequest("Kindly fill Name, Description fields!");
        }

        var dbTag = await uow.TagRepository.GetTagById(identityService.GetUserId(), dto.Id);

        if (dbTag is not null)
        {
            uow.TagRepository.UpdateTag(dbTag, dto.WithUserId(identityService.GetUserId()));
        }

        if (await uow.SaveAsync() > 0) return Ok("Your tag is updated successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTagDetails(Guid id, [FromServices] IIdentityService identityService,
        [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Please provide a valid id!");
        }

        var result = await uow.TagRepository.GetTagById(identityService.GetUserId(), id);

        return result is not null ? Ok(result) : new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTag(Guid id, [FromServices] IUnitOfWork uow)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("Please provide a valid id!");
        }

        await uow.TagRepository.DeleteTagAsync(id);

        if (await uow.SaveAsync() > 0) return Ok("Your tag is deleted successfully!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListTag([FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService)
    {
        var result = await uow.TagRepository.GetListTagAsync(identityService.GetUserId());

        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }
    
    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromQuery] string keyword)
    {
        var result = await uow.TagRepository.Search(identityService.GetUserId(), keyword);
        
        return result.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}