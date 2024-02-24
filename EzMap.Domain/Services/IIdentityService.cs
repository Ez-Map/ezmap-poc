namespace EzMap.Domain.Services;

public interface IIdentityService
{
    public Guid GetUserId();

    public Task<List<string>> VerifyPassword(string inputtedPassword);
}