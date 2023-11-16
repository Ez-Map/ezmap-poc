using Microsoft.VisualBasic;

namespace EzMap.Domain.Models;

public class Tag : EntityBase<Guid>
{
    public Tag(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    
    public string Description { get; set; }

    public List<PoiCollection> Collections { get; } = new();

    public Tag Parent { get; set; }
}

