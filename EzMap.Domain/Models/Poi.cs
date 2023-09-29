namespace EzMap.Domain;

public class Poi
{
    public Poi(string name, string address)
    {
        Id = Guid.NewGuid();
        Address = address;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}