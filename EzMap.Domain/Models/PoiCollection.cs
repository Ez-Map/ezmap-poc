using EzMap.Domain.Constants;

namespace EzMap.Domain.Models;

public class PoiCollection : EntityBase<Guid>
{
    public PoiCollection(string name, string description, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        UserId = userId;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public PoiEnum.ViewType ViewType { get; set; } = PoiEnum.ViewType.List;
    public List<Poi> Pois { get; } = new();
    public List<Tag> Tags { get; } = new();

    public Guid UserId { get; set; }

    public User User { get; set; }
}