namespace EzMap.Domain.Dtos;

public class TagCreateDto
{
    public TagCreateDto(string description, string name)
    {
        Description = description;
        Name = name;
    }

    public string Description { get; set; }

    public string Name { get; set; }

    public Guid UserId { get; private set; }

    public TagCreateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    } 
}