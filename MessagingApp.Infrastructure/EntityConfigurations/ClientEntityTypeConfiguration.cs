using MessagingApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingApp.Infrastructure.EntityConfigurations;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id);
        builder.Property(x => x.Id).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(c => c.Name).IsUnicode().IsRequired().HasMaxLength(400);
    }
}