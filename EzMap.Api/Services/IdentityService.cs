using System.Security.Claims;
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
}