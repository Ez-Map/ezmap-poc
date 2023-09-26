using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Repositories;

public interface IPointOfInterests
{
    public List<Poi> GetLstOfPois();
    public Poi GetPoiDetail(int id);
    public void AddPoi(Poi poi);
    public void UpdatePoi(Poi poi);
    public Poi DeletePoi(int id);
    public bool CheckPoi(int id);
}


public class PoiRepository : IPointOfInterests
{
    private readonly EzMapContext _dbContext ;

    public PoiRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }
        
    public List<Poi> GetLstOfPois()
    {
        return _dbContext.Pois.ToList();
    }

    public Poi GetPoiDetail(int id)
    {
        try
        {
            Poi? poi = _dbContext.Pois.Find(id);
            if (poi != null)
            {
                return poi;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        catch
        {
            throw;
        }
    }

    public void AddPoi(Poi poi)
    {
        try
        {
            _dbContext.Pois.Add(poi);
            _dbContext.SaveChanges();

        }
        catch
        {
            throw;
        }
    }

    public void UpdatePoi(Poi poi)
    {
        try
        {
            _dbContext.Entry(poi).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        catch
        {
            throw;
        }
    }

    public Poi DeletePoi(int id)
    {
        try
        {
            var poi = _dbContext.Pois.Find(id);
            if (poi != null)
            {
                _dbContext.Pois.Remove(poi);
                _dbContext.SaveChanges();
                return poi;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        catch
        {
            throw;
        }
    }

    public bool CheckPoi(int id)
    {
        return _dbContext.Pois.Any(p => p.PoiId == id);
    }
}