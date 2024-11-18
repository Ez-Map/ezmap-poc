using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using EzMap.Api.Services;
using EzMap.Domain.Dtos;
using EzMap.Domain.Indexes;
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
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();
            try
            {
                var poiCollectionId = uow.PoiCollectionRepository.AddPoiCollection(dto.WithUserId(identityService.GetUserId()));
                var dbResult = await uow.SaveAsync();
                if (dbResult <= 0)
                {
                    await transaction.RollbackAsync();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                var poiCollectionCreateIndex = new PoiCollectionCreateIndexingModel(poiCollectionId, dto.Name, dto.Description);
                var esResult = await elasticSearchService.AddOrUpdate(poiCollectionCreateIndex);
                if (!esResult)
                {
                    await transaction.RollbackAsync();
                    return Ok(new
                    {
                        Message = "Failed to index the POI collection. The operation has been rolled back.",
                        name = dto.Name
                    });
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your poi collection is created and indexed successfully!",
                    Name = dto.Name,
                    Id = poiCollectionId,
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
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
        return result?.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] PoiCollectionUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();
            try
            {
                var dbPoiCol = await uow.PoiCollectionRepository.GetPoiCollectionById(identityService.GetUserId(), dto.Id);

                if (dbPoiCol is not null)
                {
                    uow.PoiCollectionRepository.UpdatePoiCollectionAsync(dbPoiCol, dto);
                    var dbResult = await uow.SaveAsync();
                    if (dbResult <= 0)
                    {
                        await transaction.RollbackAsync();
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }

                    var poiColUpdateIndex = new PoiCollectionUpdateIndexingModel(dto.Id, dto.Name, dto.Description);
                    var esResult = await elasticSearchService.AddOrUpdate(poiColUpdateIndex);
                    if (!esResult)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            "Failed to index the POI collection. The operation has been rolled back.");
                    }
                }

                await transaction.CommitAsync();
                return Ok("Your poi collection is updated successfully");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IUnitOfWork uow,
        [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return BadRequest("Please provide a valid id!");
                }

                await uow.PoiCollectionRepository.DeletePoiCollectionAsync(id);
                var dbResult = await uow.SaveAsync();
                if(dbResult <= 0){
                    await transaction.RollbackAsync();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                var esResult = await elasticSearchService.Remove(id.ToString());
                if (!esResult)
                {
                    await transaction.RollbackAsync();
                    return Ok(new
                    {
                        Message = "Failed to remove POI collection from ElasticSearch. The operation has been rolled back.",
                    });
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your poi collection is deleted successfully!",
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListPoiCollection([FromServices] IUnitOfWork uow,
        [FromServices] IdentityService identityService)
    {
        var result = await uow.PoiCollectionRepository.GetListPoiCollectionAsync(identityService.GetUserId());

        return result?.Count > 0 ? Ok(result) : new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}