using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street)
                   .IsRequired();

            builder.Property(a => a.City)
                .IsRequired();

            builder.Property(a => a.State)
                .IsRequired();

            builder.ToTable("Addresses");
        }
    }
}
