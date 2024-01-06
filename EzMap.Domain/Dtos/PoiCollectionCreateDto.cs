namespace EzMap.Domain.Dtos;

public class PoiCollectionCreateDto
{
    public PoiCollectionCreateDto(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public string Name { get; set; }
    
    public string Description { get; init; }
    
    public Guid UserId { get; private set; }

    public PoiCollectionCreateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}