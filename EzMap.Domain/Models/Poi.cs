using EzMap.Domain.Models;

namespace EzMap.Domain;

public class Poi : EntityBase<Guid>
{
    public Poi(string name, string address)
    {
        Id = Guid.NewGuid();
        Address = address;
        Name = name;
    }
    
    public string Address { get; set; }
    public string? Name { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}