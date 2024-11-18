using System.Security.Claims;
using EzMap.Api.Services;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Indexes;
using EzMap.Domain.Repositories;
using EzMap.Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace EzMap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PoiController : ControllerBase
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] PoiCreateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        try
        {
            await using var transaction = await uow.BeginTransactionAsync();
            try 
            {
                var poiId = uow.PoiRepository.AddPoi(dto.WithUserId(identityService.GetUserId()));
                var dbResult = await uow.SaveAsync();
                if (dbResult <= 0)
                {
                    await transaction.RollbackAsync();
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                var poiCreateIndex = new PoiCreateIndexingModel(poiId, dto.Name, dto.Address);
                var esResult = await elasticSearchService.AddOrUpdate(poiCreateIndex);
                if (!esResult)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        "Failed to index the POI. The operation has been rolled back.");
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your point of interest is created and indexed successfully!",
                    Name = dto.Name,
                    Id = poiId,
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
    public async Task<IActionResult> Update([FromBody] PoiUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        var dbPoi = await uow.PoiRepository.GetPoiById(identityService.GetUserId(), dto.Id);

        if (dbPoi is not null)
        {
            uow.PoiRepository.UpdatePoiAsync(dbPoi, dto.WithUserId(identityService.GetUserId()));
            var poiUpdateIndex = new PoiUpdateIndexingModel(dbPoi.Id, dbPoi.Name, dbPoi.Address);
            await elasticSearchService.AddOrUpdate(poiUpdateIndex);
        }

        return await uow.SaveAsync() > 0
            ? Ok("Your point of interest is updated successfully!")
            : new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IUnitOfWork uow,
        [FromServices] IElasticSearchService elasticSearchService)
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
                await uow.PoiRepository.DeletePoiAsync(id);
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
                        "Failed to remove POI from ElasticSearch. The operation has been rolled back.");
                }

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Your point of interest is deleted successfully!",
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