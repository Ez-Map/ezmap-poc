namespace EzMap.Domain.Models;

public class Itinerary : EntityBase<Guid>
{
    public Itinerary(string name)
    {
        
    }
    
    public String Name { get; set; }

    public List<Poi> Pois { get; } = new List<Poi>();
}