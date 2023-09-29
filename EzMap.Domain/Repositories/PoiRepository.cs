using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPointOfInterests
{
    public void AddPoi(Poi poi);
}


public class PoiRepository : IPointOfInterests
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