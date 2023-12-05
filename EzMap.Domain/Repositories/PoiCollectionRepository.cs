using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EzMap.Domain.Repositories;

public interface IPoiCollectionRepository
{
    Task<PoiCollection?> GetPoiCollectionById(Guid? userId, Guid id, CancellationToken token = default);

    Task<List<PoiCollection>?> GetListPoiCollectionAsync(Guid? userId, CancellationToken token = default);

    void AddPoiCollection(PoiCollectionCreateDto dto);

    Task DeletePoiCollectionAsync(Guid id, CancellationToken token = default);

    Task<List<PoiCollection>?> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class PoiCollectionRepository : IPoiCollectionRepository
{
    private readonly EzMapContext _dbContext;

    public PoiCollectionRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PoiCollection?> GetPoiCollectionById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var poi = await _dbContext.PoiCollections
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken: token);

        return poi;
    }

    public async Task<List<PoiCollection>?> GetListPoiCollectionAsync(Guid? userId, CancellationToken token = default)
    {
        if (userId == null) return null;
        List<PoiCollection> poiCollections = await _dbContext.PoiCollections
            .Where(x => x.UserId == userId).ToListAsync(cancellationToken: token);

        return poiCollections;
    }

    public void AddPoiCollection(PoiCollectionCreateDto dto)
    {
        var poiCollection = new PoiCollection(dto.Name, dto.Description);

        _dbContext.PoiCollections.Add(poiCollection);
    }

    public void UpdatePoiCollectionAsync(PoiCollection dbPoiCollection, PoiCollectionUpdateDto dto,
        CancellationToken token = default)
    {
        dbPoiCollection.Name = dto.Name;
        dbPoiCollection.Description = dto.Description;
        dbPoiCollection.ViewType = dto.ViewType;
        dbPoiCollection.Tags.Clear();
        dbPoiCollection.Tags.AddRange(dto.Tags);
        dbPoiCollection.Pois.Clear();
        dbPoiCollection.Pois.AddRange(dto.Pois);
    }

    public async Task DeletePoiCollectionAsync(Guid id, CancellationToken token = default)
    {
        PoiCollection? poiCollection = await _dbContext.PoiCollections.SingleOrDefaultAsync(pc => pc.Id == id, token);
        if (poiCollection is not null)
        {
            poiCollection.DeletedDate = DateTime.Now;
        }
    }

    public async Task<List<PoiCollection>?> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var poiCollection = _dbContext.PoiCollections
            .Include(poiCollection => poiCollection.Tags)
            .Include(poiCollection => poiCollection.Pois)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            poiCollection = poiCollection.Where(x => x.UserId == userId);

            poiCollection = poiCollection.Where(
                x => (x.Name.ToLower().Contains(keyword.ToLower()) ||
                      x.Description.ToLower().Contains(keyword.ToLower()))
                     || x.Pois.Exists(x => x.Name.ToLower().Contains(keyword.ToLower()))
                     || x.Tags.Exists(x => x.Name.ToLower().Contains(keyword.ToLower()))
            );
        }

        return await poiCollection.ToListAsync(cancellationToken: token);
    }
}