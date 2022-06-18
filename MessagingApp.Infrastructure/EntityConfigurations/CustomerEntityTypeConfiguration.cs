using MessagingApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingApp.Infrastructure.EntityConfigurations;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.FirstName).IsUnicode().HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.LastName).IsUnicode().HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.Msisdn).IsRequired().HasMaxLength(20).IsUnicode(false);

        builder.HasIndex(x => x.Msisdn).IsUnique();
    }
}