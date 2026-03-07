using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    Guid AddPoi(PoiCreateDto dto, CancellationToken token = default);

    Task<List<Poi>?> GetListPoiAsync(Guid? userId, CancellationToken token = default);

    void UpdatePoiAsync(Poi dbPoi,PoiUpdateDto dto);

    Task DeletePoiAsync(Guid id, CancellationToken token = default);

    Task<Poi?> GetPoiById(Guid? userId, Guid id, CancellationToken token = default);

    Task<List<Poi>?> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext;
    private ILogger<PoiRepository> _logger;

    public PoiRepository(EzMapContext dbContext, ILogger<PoiRepository> logger)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Poi?> GetPoiById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var poi = await _dbContext.Pois.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId,
            cancellationToken: token);
        
        _logger.LogInformation($"Returned Poi with Poi Id: {id}, belonged to user with Id: {userId}");
        
        return poi;
    }

    public Guid AddPoi(PoiCreateDto dto, CancellationToken token = default)
    {
        Poi poi = new Poi(dto.Name, dto.Address, dto.UserId);
        
        _dbContext.Pois.AddAsync(poi, token);

        _logger.LogInformation($"Prepared new Poi to add: {poi.Name}, ({poi.Id}) for User ID: {poi.UserId}");

        return poi.Id;
    }

    public void UpdatePoiAsync(Poi dbPoi,PoiUpdateDto dto)
    {
        dbPoi.Name = dto.Name;
        dbPoi.Address = dto.Address;
        
        _logger.LogInformation($"$Poi: {dto.Id} is updated with new data");
    }

    public async Task DeletePoiAsync(Guid id, CancellationToken token = default)
    {
        Poi? poi = await _dbContext.Pois.SingleOrDefaultAsync(p => p.Id == id, cancellationToken: token);

        if (poi is not null)
        {
            poi.DeletedDate = DateTime.Now;
            _logger.LogInformation($"Deleted Poi: {poi.Name} ({poi.Id})");
        }
    }

    public async Task<List<Poi>?> GetListPoiAsync(Guid? userId, CancellationToken token = default)
    {
        if (userId == null) return null;
        List<Poi>? listPoi = await _dbContext.Pois.Where(
            x => x.UserId == userId).ToListAsync(cancellationToken: token);

        _logger.LogInformation($"Returned a list of {listPoi.Count} pois for User ID: {userId}");
        return listPoi;
    }

    public async Task<List<Poi>?> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var pois = new List<Poi>();

        if (!String.IsNullOrEmpty(keyword))
        {
            pois = await _dbContext.Pois.Where(x => (x.Address.ToLower().Contains(keyword.ToLower()) || x.Name.ToLower().Contains(keyword.ToLower())) && x.UserId == userId ).ToListAsync(token);
        }

        if (!pois.Any())
        {
            _logger.LogInformation($"There is no matched poi searched with Keyword: {keyword} (UserId : {userId})");
        }
        
        _logger.LogInformation($"{pois.Count} matched poi(s) searched with Keyword: {keyword} (UserId : {userId})");

        return pois;
    }
}