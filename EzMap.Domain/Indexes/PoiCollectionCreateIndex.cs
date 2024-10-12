namespace EzMap.Domain.Indexes;

public class PoiCollectionCreateIndex
{
    public PoiCollectionCreateIndex(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public string Description { get; set; }

    public string Name { get; set; }

    public Guid Id { get; set; }
}