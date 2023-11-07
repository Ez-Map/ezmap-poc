using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzMap.Domain.Models;

public class ItineraryConfiguration : IEntityTypeConfiguration<Itinerary>
{
    public void Configure(EntityTypeBuilder<Itinerary> builder)
    {

        builder.ToTable(nameof(Itinerary)).HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
    }
}