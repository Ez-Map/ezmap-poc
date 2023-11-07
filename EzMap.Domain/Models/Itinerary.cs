namespace EzMap.Domain.Models;

public class Itinerary : EntityBase<Guid>
{
    public Itinerary(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    public List<Poi> Pois { get; } = new List<Poi>();
}