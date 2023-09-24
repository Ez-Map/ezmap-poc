using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzMap.Domain;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasNoKey();
        builder.ToTable("User");
        builder.Property(u => u.UserId).HasColumnName("UserId");
        builder.Property(u => u.DisplayName).HasMaxLength(60).IsUnicode(false);
        builder.Property(u => u.UserName).HasMaxLength(30).IsUnicode(false);
        builder.Property(u => u.Email).HasMaxLength(50).IsUnicode(false);
        builder.Property(u => u.Password).HasMaxLength(20).IsUnicode(false);
        builder.Property(u => u.CreatedDate).IsUnicode(false);

        builder.HasMany(u => u.SelectedPois)
            .WithOne(p => p.User);
    }
}