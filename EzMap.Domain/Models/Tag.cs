using Microsoft.VisualBasic;

namespace EzMap.Domain.Models;

public class Tag : EntityBase<Guid>
{
    public Tag(string name, string description, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        UserId = userId;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public List<PoiCollection> Collections { get; } = new();
    public Guid? ParentId { get; set; }
    public Tag Parent { get; set; }
    public List<Tag> Tags { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}