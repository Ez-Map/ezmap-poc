using EzMap.Api.Services;
using EzMap.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;
[ApiController]
public class PoiElasticSearchController : ElasticSearchController<Poi>
{
    
    public PoiElasticSearchController(IElasticSearchService<Poi> elasticSearchService) : base(elasticSearchService)
    {
    }
}