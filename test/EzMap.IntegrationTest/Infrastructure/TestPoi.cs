using EzMap.Domain;

namespace EzMap.IntegrationTest.Infrastructure;

public class TestPoi
{
    public static readonly Poi DefaultPoi = new Poi("Test Poi", "Test Poi Name", TestUser.DefaultUser.Id);
}