namespace EzMap.Domain;

public class User
{
    public int UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime? CreatedDate { get; set; }
    
    public List<Poi> SelectedPois { get; } = new List<Poi>();
}