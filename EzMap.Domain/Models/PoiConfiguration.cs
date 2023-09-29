﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzMap.Domain;

internal class PoiConfiguration :  IEntityTypeConfiguration<Poi>
{
    public void Configure(EntityTypeBuilder<Poi> builder)
    {
        builder.ToTable(nameof(Poi)).HasKey(p => p.Id);
        builder.Property(p => p.Address).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).ValueGeneratedOnAdd();
        builder.HasOne(p => p.User).WithMany(u => u.SelectedPois).HasForeignKey(p => p.UserId);
    }
}