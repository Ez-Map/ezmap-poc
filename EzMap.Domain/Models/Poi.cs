namespace EzMap.Domain;

public class Poi
{
    public Poi(string address, string name, Guid userId, User? user)
    {
        Id = Guid.NewGuid();
        Address = address;
        Name = name;
        UserId = userId;
        User = user;
    }

    public Guid Id { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}