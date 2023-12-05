namespace EzMap.Domain.Dtos;

public class TagUpdateDto
{
    public TagUpdateDto(Guid id, string description, string name)
    {
        Id = id;
        Description = description;
        Name = name;
    }
    
    public Guid Id { get; set; }
    
    public string Description { get; set; }
    
    public string Name { get; set; }
    
    public Guid UserId { get; private set; }

    public TagUpdateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}