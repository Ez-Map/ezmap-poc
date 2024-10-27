using System.Text;
using EzMap.Api.Services;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.Domain.Repositories;
using EzMap.Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();



Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .CreateLogger();

builder.Logging.AddSerilog();

// Add services to the container.
builder.Services.AddDbContext<EzMapContext>(
    options => { options.UseSqlServer(builder.Configuration.GetConnectionString("myDb1")); }
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PoiCreateDtoValidator>();
// builder.Environment.EnvironmentName = "Test";
// Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
var appSettings = configuration["AppSecret"];
var key = Encoding.ASCII.GetBytes(appSettings);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new[]
            {
                "Bearer"
            }
        }
    });
});

builder.Services.AddHttpContextAccessor();

if (!environmentName.Equals("Test"))
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        dbContext.Database.Migrate();
    }
    
    var elasticsearchUrl = configuration["ELK:URl"]; // Replace with your Elasticsearch connection string

    if (string.IsNullOrEmpty(elasticsearchUrl))
    {
        throw new InvalidOperationException("Elasticsearch URL is not configured.");
    }

    var settings = new ConnectionSettings(new Uri(elasticsearchUrl)) 
        .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true)
        .BasicAuthentication("elastic1", "p1vrgOeVbPfN=YOHhOD" +
                                         "a")
        .EnableApiVersioningHeader();
    var client = new ElasticClient(settings);

    builder.Services.AddSingleton<IElasticClient>(client);
    builder.Services.AddSingleton<IElasticSearchService, ElasticSearchService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program
{
}