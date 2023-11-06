namespace EzMap.Domain.Models;

public class Collection : EntityBase<Guid>
{
    public Collection(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    
    public string Name { get; set; }
    
    public List<Poi> Pois { get; } = new();
}              