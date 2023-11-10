namespace EzMap.Domain.Models;

public class User : EntityBase<Guid>
{
    public User(string displayName, string userName, string email, string password)
    {
        Id = Guid.NewGuid();
        DisplayName = displayName;
        UserName = userName;
        Email = email;
        Password = password;
    }

    public string DisplayName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public List<Poi> SelectedPois { get; } = new List<Poi>();
}