using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    Task<bool> AddPoi(PoiCreateDto dto);

    Task ReadBooksAsync(CancellationToken token = default);
}


public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext ;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ReadBooksAsync(CancellationToken token = default)
    {
        string query = _dbContext.Pois.ToQueryString();
        Console.WriteLine();
        List<Poi> pois = await _dbContext.Pois.ToListAsync(token);
        foreach (var p in pois)
        {
            Console.WriteLine($"{p.Address} {p.Name}");
        }
        
        Console.WriteLine();
    }

    public async Task<bool> AddPoi(PoiCreateDto dto)
    {
        Poi poi = new Poi(dto.Name, dto.Address) ;
        await _dbContext.Pois.AddAsync(poi);
        int records = await _dbContext.SaveChangesAsync();
        Console.WriteLine($"{records} record added with {poi.Id}");
        
        return records > 0;
    }
}