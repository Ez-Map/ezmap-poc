using EzMap.Domain.Constants;

namespace EzMap.Domain.Models;

public class PoiCollection : EntityBase<Guid>
{
    public PoiCollection(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public PoiEnum.ViewType ViewType { get; set; } = PoiEnum.ViewType.List;
    public List<Poi> Pois { get; } = new();
    public List<Tag> Tags { get; } = new();
}