using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzMap.Domain.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User)).HasKey(u => u.Id);
        builder.Property(u => u.Id);
        builder.Property(u => u.DisplayName);
        builder.Property(u => u.UserName).IsUnicode(false);
        builder.Property(u => u.Email);
        builder.Property(u => u.Password);
        builder.Property(u => u.CreatedDate);

        builder.HasMany(u => u.SelectedPois)
            .WithOne(p => p.User);

        builder.HasQueryFilter(u => u.DeletedDate == null);
    }
}