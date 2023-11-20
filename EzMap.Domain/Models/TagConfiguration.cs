using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;

namespace EzMap.Domain.Models;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(nameof(Tag)).HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        builder.Property(x => x.Parent);
        builder.HasMany<PoiCollection>(x => x.Collections)
            .WithMany(x => x.Tags);
    }
}