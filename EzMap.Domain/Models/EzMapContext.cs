using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Models;

public class EzMapContext : DbContext
{
    public DbSet<Poi>? Pois { get; set; }

    public DbSet<User>? Users { get; set; }

    public DbSet<PoiCollection>? PoiCollections { get; set; }

    public DbSet<Tag>? Tags { get; set; }

    public EzMapContext(DbContextOptions<EzMapContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new PoiConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new PoiCollectionConfiguration())
            .ApplyConfiguration(new TagConfiguration());
        //…
    }
}