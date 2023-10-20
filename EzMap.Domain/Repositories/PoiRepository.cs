using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    void AddPoi(PoiCreateDto dto);

    Task<List<Poi>> GetListPoiAsync(CancellationToken token = default);

    Task UpdatePoiAsync(PoiUpdateDto dto);

    Task DeletePoiAsync(Guid id);

    Task<Poi?> GetPoiById(Guid id, CancellationToken token = default);
}

public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Poi?> GetPoiById(Guid id, CancellationToken token = default)
    {
        Console.WriteLine();
        var poi = await _dbContext.Pois.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
        return poi;
    }

    public async Task<List<Poi>> GetListPoiAsync(CancellationToken token = default)
    {  
        Console.WriteLine();
        List<Poi> poi = await _dbContext.Pois.ToListAsync(token);


        return poi;
    }

    public void AddPoi(PoiCreateDto dto)
    {
        Poi poi = new Poi(dto.Name, dto.Address, dto.UserId);
        _dbContext.Pois.Add(poi);
    }

    public async Task UpdatePoiAsync(PoiUpdateDto dto)
    {
        Poi? poi = await _dbContext.Pois.FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (poi != null)
        {
            poi.Name = dto.Name;
            poi.Address = dto.Address;
        }
    }

    public async Task DeletePoiAsync(Guid id)
    {
        Poi? poi = await _dbContext.Pois.SingleOrDefaultAsync(p => p.Id == id);

        if (poi != null)
        {
            poi.DeletedDate = DateTime.Now;
        }
    }
}