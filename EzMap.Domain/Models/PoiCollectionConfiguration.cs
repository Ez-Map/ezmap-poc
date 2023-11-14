using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzMap.Domain.Models;

public class PoiCollectionConfiguration : IEntityTypeConfiguration<PoiCollection>
{
    public void Configure(EntityTypeBuilder<PoiCollection> builder)
    {

        builder.ToTable(nameof(PoiCollection)).HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        builder.Property(x => x.DefaultViewType);
    }
}