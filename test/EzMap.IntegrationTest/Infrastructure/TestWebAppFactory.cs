using System.Data.Common;
using EzMap.Api.Services;
using EzMap.Domain;
using EzMap.Domain.Indexes;
using EzMap.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Nest;

namespace EzMap.IntegrationTest.Infrastructure;

public class TestWebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EzMapContext>();
            db.Database.EnsureCreated();
            db.Users.Add(TestUser.DefaultUser);
            db.SaveChanges();
        }

        host.Start();
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<EzMapContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<EzMapContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
            
            // Add a mocked IElasticSearchService
            var mockElasticSearchService = new Mock<IElasticSearchService>();

            // Set up mock behavior here as needed
            mockElasticSearchService
                .Setup(es => es.AddOrUpdate(It.IsAny<PoiCreateIndex>()))
                .ReturnsAsync(true);
            
            mockElasticSearchService
                .Setup(es => es.AddOrUpdate(It.IsAny<TagCreateIndex>()))
                .ReturnsAsync(true);
            
            mockElasticSearchService
                .Setup(es => es.AddOrUpdate(It.IsAny<PoiCollectionCreateIndex>()))
                .ReturnsAsync(true);

            mockElasticSearchService
                .Setup(es => es.Remove(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockElasticSearchService
                .Setup(es => es.Query(It.IsAny<QueryContainer>())).ReturnsAsync(new List<object>
                {
                    () =>
                    {
                        new List<object>();
                    }
                });

            services.AddSingleton(mockElasticSearchService.Object);
        });

        builder.ConfigureAppConfiguration((ctx, builder) => { builder.AddJsonFile("appsettings.json"); });

        builder.UseEnvironment("Development");
    }
}