using System.Runtime.InteropServices.ComTypes;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPoiRepository
{
    public void AddPoi(Poi poi);
}


public class PoiRepository : IPoiRepository
{
    private readonly EzMapContext _dbContext ;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddPoi(Poi poi)
    {
        
    }
}