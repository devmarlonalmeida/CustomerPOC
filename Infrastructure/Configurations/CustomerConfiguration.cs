using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => c.Email)
            .IsUnique();

            builder.Property(c => c.Email)
                .IsRequired();

            builder.Property(c => c.Logo)
                   .HasColumnType("VARBINARY(MAX)")
                   .IsRequired(false);

            builder.Property(c => c.LogoFileName)
                .IsRequired();

            builder.Property(c => c.LogoContentType)
                .IsRequired();

            builder.HasMany(c => c.Addresses)
                   .WithOne()
                   .HasForeignKey("CustomerId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Customers");
        }
    }
}
