using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    void AddPoi(PoiCreateDto dto);

    Task ReadPoisAsync(CancellationToken token = default);
}


public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext ;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ReadPoisAsync(CancellationToken token = default)
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

    public void AddPoi(PoiCreateDto dto)
    {
        Poi poi = new Poi(dto.Name, dto.Address, dto.UserId) ; 
        _dbContext.Pois.Add(poi);
    }
}