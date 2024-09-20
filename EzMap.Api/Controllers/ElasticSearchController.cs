using EzMap.Api.Services;
using EzMap.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace EzMap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElasticSearchController<T> : ControllerBase where T : class
{
    private readonly IElasticSearchService<T> _elasticSearchService;

    public ElasticSearchController(IElasticSearchService<T> elasticSearchService)
    {
        _elasticSearchService = elasticSearchService;
    }
     
    [Authorize]
    [HttpPost("index/{indexName}")]
    public async Task<IActionResult> CreateIndex(string indexName)
    {
        await _elasticSearchService.CreateIndexIfNotExists(indexName);
        return Ok();
    }

  //  [Authorize]
    [HttpPost("bulk")]
    public async Task<IActionResult> AddOrUpdateBulk([FromBody] IEnumerable<T> documents)
    {
        var result = await _elasticSearchService.AddOrUpdateBulk(documents);
        return result ? Ok() : BadRequest();
    }

  //  [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> AddOrUpdate([FromBody] T document)
    {
        var result = await _elasticSearchService.AddOrUpdate(document);
        return result ? Ok() : BadRequest();
    }

    [Authorize]
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var document = await _elasticSearchService.Get(key);
        return Ok(document);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _elasticSearchService.GetAll();
        return documents != null ? Ok(documents) : NotFound();
    }

    [Authorize]
    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] QueryContainer predicate)
    {
        var documents = await _elasticSearchService.Query(predicate);
        return documents != null ? Ok(documents) : NotFound();
    }

    [Authorize]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Remove(string key)
    {
        var result = await _elasticSearchService.Remove(key);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveAll()
    {
        var deletedCount = await _elasticSearchService.RemoveAll();
        return Ok(deletedCount);
    }
}