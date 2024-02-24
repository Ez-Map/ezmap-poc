using System.Security.Claims;
using System.Text.RegularExpressions;
using EzMap.Domain.Services;

namespace EzMap.Api.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public Guid GetUserId()
    {
        _ = Guid.TryParse(_httpContextAccessor?.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);

        return tempId;
    }

    public async Task<List<string>> VerifyPassword(string inputtedPassword)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(inputtedPassword))
        {
            errors.Add("Password is required.");
        }

        if (inputtedPassword.Length < 8 || inputtedPassword.Length > 16)
        {
            errors.Add("Password must be between 8 and 16 characters long.");
        }

        if (!Regex.IsMatch(inputtedPassword, @"[A-Z]"))
        {
            errors.Add("Password must contain at least one uppercase letter.");
        }

        if (!Regex.IsMatch(inputtedPassword, @"[a-z]"))
        {
            errors.Add("Password must contain at least one lowercase letter.");
        }

        if (!Regex.IsMatch(inputtedPassword, @"\d"))
        {
            errors.Add("Password must contain at least one digit.");
        }

        if (!Regex.IsMatch(inputtedPassword, @"[!@#$%^&*()\-+=<>?\|~`{}]"))
        {
            errors.Add("Password must contain at least one special character.");
        }

        return errors;
    }
}