namespace EzMap.Domain;

public class User
{
    public User(string displayName, string userName, string email, string password, DateTime createdDate)
    {
        Id = Guid.NewGuid();
        DisplayName = displayName;
        UserName = userName;
        Email = email;
        Password = password;
        CreatedDate = createdDate;
    }

    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedDate { get; set; }
    
    public List<Poi> SelectedPois { get; } = new List<Poi>();
}