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


    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] PoiUpdateDto dto, [FromServices] IUnitOfWork uow,
        [FromServices] IIdentityService identityService, [FromServices] IElasticSearchService elasticSearchService)
    {
        await using var transaction = await uow.BeginTransactionAsync();
        try
        {
            var dbPoi = await uow.PoiRepository.GetPoiById(identityService.GetUserId(), dto.Id);

            if (dbPoi is null)
            {
                return NotFound("Poi not found.");
            }

            uow.PoiRepository.UpdatePoiAsync(dbPoi, dto);
            var dbResult = await uow.SaveAsync();
            if (dbResult == 0)
            {
                Console.WriteLine("No changes detected. Skipping commit.");
                return Ok("No changes were made to the POI collection.");
            }

            var poiUpdateIndex = new PoiUpdateIndexingModel(dbPoi.Id, dto.Name, dto.Address);
            var esResult = await elasticSearchService.AddOrUpdate(poiUpdateIndex);
            if (!esResult)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to index the POI. The operation has been rolled back.");
            }

            await transaction.CommitAsync();
            return Ok("Your poi is updated successfully!");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IUnitOfWork uow,
        [FromServices] IElasticSearchService elasticSearchService)
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
                return NotFound("Poi not found.");
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