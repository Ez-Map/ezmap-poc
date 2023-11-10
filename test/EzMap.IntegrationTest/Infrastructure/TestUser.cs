using EzMap.Domain;
using EzMap.Domain.Models;

namespace EzMap.IntegrationTest.Infrastructure;

public class TestUser
{
    public static readonly User DefaultUser = new User("string", "string", "string", BCrypt.Net.BCrypt.HashPassword("string"));
}