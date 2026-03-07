using EzMap.Domain.Models;

namespace EzMap.IntegrationTest.Infrastructure;

public class TestPoiCollection
{
    public static readonly PoiCollection DefaultPoiCollection = new PoiCollection("Poi Col Test", "59 Ngo Tat To", TestUser.DefaultUser.Id);
}