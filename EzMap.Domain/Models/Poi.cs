namespace EzMap.Domain;

public class Poi
{
    public int PoiId { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
}