using Microsoft.EntityFrameworkCore;

namespace EzMap.Domain;

public class EzMapContext : DbContext
{
    public DbSet<Poi> Pois { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public EzMapContext(DbContextOptions<EzMapContext> options)
        : base(options)
    {
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mc")
            .ApplyConfiguration(new PoiConfiguration())
            .ApplyConfiguration(new UserConfiguration());        
        //…
    }
    
}