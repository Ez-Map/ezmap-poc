using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain.Models;

public class EzMapContext : DbContext
{
    public DbSet<Poi> Pois => Set<Poi>();
    
    public DbSet<User> Users => Set<User>();
    
    public EzMapContext(DbContextOptions<EzMapContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new PoiConfiguration())
            .ApplyConfiguration(new UserConfiguration());        
        //…
    }
    
}