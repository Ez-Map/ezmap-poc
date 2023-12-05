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

        builder.HasOne<Tag>(x => x.Parent)
            .WithMany(x => x.Tags).HasForeignKey(x => x.ParentId).IsRequired(false);

        builder.HasMany<Tag>(x => x.Tags)
            .WithOne(x => x.Parent).HasForeignKey(x => x.ParentId);

        builder.HasOne<User>(x => x.User)
            .WithMany(x => x.SelectedTags).HasForeignKey(x => x.UserId);


        builder.HasMany<PoiCollection>(x => x.Collections)
            .WithMany(x => x.Tags);
        
        builder.HasQueryFilter(x => x.DeletedDate == null);
    }
}