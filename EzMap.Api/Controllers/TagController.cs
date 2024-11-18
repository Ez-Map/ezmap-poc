using EzMap.Api.Services;
using EzMap.Domain.Dtos;
using EzMap.Domain.Indexes;
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
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();
            try
            {
                var tagId = uow.TagRepository.AddTag(dto.WithUserId(identityService.GetUserId()));
                var dbResult = await uow.SaveAsync();
                if (dbResult <= 0)
                {
                    await transaction.RollbackAsync();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                var tagCreateIndex = new TagCreateIndexingModel(tagId, dto.Name, dto.Description);
                var esResult = await elasticSearchService.AddOrUpdate(tagCreateIndex);
                if (!esResult)
                {
                    await transaction.RollbackAsync();
                    return Ok(new
                    {
                        Message = "Failed to index the tag. The operation has been rolled back.",
                        name = dto.Name
                    });
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your tag is created and indexed successfully!",
                    Name = dto.Name,
                    Id = tagId,
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();
            try
            {
                var dbTag = await uow.TagRepository.GetTagById(identityService.GetUserId(), dto.Id);

                if (dbTag is not null)
                {
                    uow.TagRepository.UpdateTag(dbTag, dto.WithUserId(identityService.GetUserId()));
                    var dbResult = await uow.SaveAsync();
                    if (dbResult <= 0)
                    {
                        await transaction.RollbackAsync();
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }

                    var tagUpdateIndex = new TagUpdateIndexingModel(dto.Id, dto.Name, dto.Description);
                    var esResult = await elasticSearchService.AddOrUpdate(tagUpdateIndex);
                    if (!esResult)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            "Failed to index the tag. The operation has been rolled back.");
                    }
                }

                await transaction.CommitAsync();
                return Ok("Your tag is updated successfully!");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
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
    public async Task<IActionResult> DeleteTag(Guid id, [FromServices] IUnitOfWork uow, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("Please provide a valid id!");
            }

            await using var transaction = await uow.BeginTransactionAsync();
            try
            {
                await uow.TagRepository.DeleteTagAsync(id);
                var dbResult = await uow.SaveAsync();
                if (dbResult <= 0)
                {
                    await transaction.RollbackAsync();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                var esResult = await elasticSearchService.Remove(id.ToString());
                if (!esResult)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Failed to remove tag from ElasticSearch. The operation has been rolled back.");
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your tag is deleted successfully!",
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
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