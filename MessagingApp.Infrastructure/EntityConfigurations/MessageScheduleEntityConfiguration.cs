using MessagingApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingApp.Infrastructure.EntityConfigurations;

public class MessageScheduleEntityConfiguration : IEntityTypeConfiguration<MessageSchedule>
{
    public void Configure(EntityTypeBuilder<MessageSchedule> builder)
    {
        builder.ToTable("MessageSchedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.CampaignId).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.ClientId).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignId);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId);
          builder.HasMany(x => x.TargetCustomers).WithMany(c => c.MessageSchedules)
                       .UsingEntity<MessageScheduleCustomer>(x =>
                        {
                            x.HasKey(y => new { MessageCampaignId = y.MessageScheduleId, y.CustomerId });
                            x.HasOne<MessageSchedule>().WithMany().HasForeignKey(y => y.MessageScheduleId);
                            x.HasOne<Customer>().WithMany().HasForeignKey(y => y.CustomerId);
                        });
          
        builder.Property(x => x.ScheduleType).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.HasDiscriminator(x => x.ScheduleType)
               .HasValue<InstantMessageSchedule>("Instant");
    }
}

public class InstantMessageScheduleEntityConfiguration : IEntityTypeConfiguration<InstantMessageSchedule>
{
    public void Configure(EntityTypeBuilder<InstantMessageSchedule> builder)
    {
        builder.Property(x => x.SendDate).IsRequired();
        builder.Property(x => x.DispatchStatus).IsRequired().IsUnicode(false).HasMaxLength(50);
    }
}