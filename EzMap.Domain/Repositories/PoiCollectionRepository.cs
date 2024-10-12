using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;

namespace EzMap.Domain.Repositories;

public interface IPoiCollectionRepository
{
    Task<PoiCollection?> GetPoiCollectionById(Guid? userId, Guid id, CancellationToken token = default);

    void UpdatePoiCollectionAsync(PoiCollection dbPoiCollection, PoiCollectionUpdateDto dto);

    Task<List<PoiCollection>?> GetListPoiCollectionAsync(Guid? userId, CancellationToken token = default);

    Guid AddPoiCollection(PoiCollectionCreateDto dto);

    Task DeletePoiCollectionAsync(Guid id, CancellationToken token = default);

    Task<List<PoiCollection>?> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class PoiCollectionRepository : IPoiCollectionRepository
{
    private readonly EzMapContext _dbContext;
    private readonly ILogger<PoiCollection> _logger;

    public PoiCollectionRepository(EzMapContext dbContext, ILogger<PoiCollection> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PoiCollection?> GetPoiCollectionById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var poi = await _dbContext.PoiCollections
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken: token);
        
        _logger.LogInformation($"Retrieved Poi: {id} of and User Id: {userId} ");

        return poi;
    }

    public async Task<List<PoiCollection>?> GetListPoiCollectionAsync(Guid? userId, CancellationToken token = default)
    {
        if (userId == null) return null;
        List<PoiCollection> poiCollections = await _dbContext.PoiCollections
            .Where(x => x.UserId == userId).ToListAsync(cancellationToken: token);
        
        _logger.LogInformation($"Retrieved a list: {poiCollections.Count} Poi Collection of User ID: {userId}");

        return poiCollections;
    }

    public Guid AddPoiCollection(PoiCollectionCreateDto dto)
    {
        var poiCollection = new PoiCollection(dto.Name, dto.Description, dto.UserId);

        _dbContext.PoiCollections?.Add(poiCollection);
        
        _logger.LogInformation($"Prepared a new Poi Collection to add: {poiCollection.Name} ({poiCollection.Id})");

        return poiCollection.Id;
    }

    public void UpdatePoiCollectionAsync(PoiCollection dbPoiCollection, PoiCollectionUpdateDto dto)
    {
        dbPoiCollection.Name = dto.Name;
        dbPoiCollection.Description = dto.Description;
        dbPoiCollection.ViewType = dto.ViewType;
        dbPoiCollection.Tags.Clear();
        dbPoiCollection.Tags.AddRange(dto.Tags);
        dbPoiCollection.Pois.Clear();
        dbPoiCollection.Pois.AddRange(dto.Pois);
        
        _logger.LogInformation($"Updated existing Poi Collection: {dbPoiCollection.Id}");
    }

    public async Task DeletePoiCollectionAsync(Guid id, CancellationToken token = default)
    {
        PoiCollection? poiCollection = await _dbContext.PoiCollections.SingleOrDefaultAsync(pc => pc.Id == id, token);
        if (poiCollection is not null)
        {
            poiCollection.DeletedDate = DateTime.Now;
            _logger.LogInformation($"Deleted PoiCollection: {poiCollection.Name} ({poiCollection.Id})");
        }
    }

    public async Task<List<PoiCollection>?> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var poiCollection = _dbContext.PoiCollections
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            poiCollection = poiCollection.Where(x => x.UserId == userId).AsQueryable();

            poiCollection = poiCollection.Where(
                x => (x.Name.ToLower().Contains(keyword.ToLower()) ||
                      x.Description.ToLower().Contains(keyword.ToLower()))
                     || x.Pois.Any(x => x.Name.ToLower().Contains(keyword.ToLower()))
                     || x.Tags.Any(x => x.Name.ToLower().Contains(keyword.ToLower()))
            );
        }
        
        _logger.LogInformation($"{poiCollection.Count()} matched poi(s) searched with Keyword: {keyword} (UserId : {userId})");

        return await poiCollection.ToListAsync(cancellationToken: token);
    }
}