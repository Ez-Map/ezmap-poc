using EzMap.Domain.Models;

namespace EzMap.IntegrationTest.Infrastructure;

public class TestTag
{
    public static readonly Tag DefaultTag = new Tag("string", "stringstring", TestUser.DefaultUser.Id);
}