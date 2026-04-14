using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioFila.Domain.Entities;

namespace PortfolioFila.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.CustomerEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
