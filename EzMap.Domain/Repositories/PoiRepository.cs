using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    void AddPoi(PoiCreateDto dto, CancellationToken token = default);

    Task<List<Poi>?> GetListPoiAsync(Guid? userId, CancellationToken token = default);

    void UpdatePoiAsync(Poi dbPoi,PoiUpdateDto dto, CancellationToken token = default);

    Task DeletePoiAsync(Guid id, CancellationToken token = default);

    Task<Poi?> GetPoiById(Guid? userId, Guid id, CancellationToken token = default);

    Task<List<Poi>?> Search(Guid? userId, string keyword, CancellationToken token = default);
}

public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Poi?> GetPoiById(Guid? userId, Guid id, CancellationToken token = default)
    {
        var poi = await _dbContext.Pois.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId,
            cancellationToken: token);
        return poi;
    }

    public void AddPoi(PoiCreateDto dto, CancellationToken token = default)
    {
        Poi poi = new Poi(dto.Name, dto.Address, dto.UserId);
        _dbContext.Pois.AddAsync(poi, token);
    }

    public void UpdatePoiAsync(Poi dbPoi,PoiUpdateDto dto, CancellationToken token = default)
    {
        dbPoi.Name = dto.Name;
        dbPoi.Address = dto.Address;
    }

    public async Task DeletePoiAsync(Guid id, CancellationToken token = default)
    {
        Poi? poi = await _dbContext.Pois.SingleOrDefaultAsync(p => p.Id == id, cancellationToken: token);

        if (poi is not null)
        {
            poi.DeletedDate = DateTime.Now;
        }
    }
    
    

    public async Task<List<Poi>?> GetListPoiAsync(Guid? userId, CancellationToken token = default)
    {
        if (userId == null) return null;
        List<Poi>? listPoi = await _dbContext.Pois.Where(
            x => x.UserId == userId).ToListAsync(cancellationToken: token);

        return listPoi;
    }

    public async Task<List<Poi>?> Search(Guid? userId, string keyword, CancellationToken token = default)
    {
        var pois = await _dbContext.Pois.ToListAsync(cancellationToken: token);

        if (!String.IsNullOrEmpty(keyword))
        {
            pois = pois.Where(x => (x.Address.ToLower().Contains(keyword.ToLower()) || x.Name.ToLower().Contains(keyword.ToLower())) && x.UserId == userId ).ToList();
        }

        return pois;
    }
}