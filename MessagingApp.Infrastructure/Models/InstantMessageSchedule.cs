using MessagingApp.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Models;

[EntityTypeConfiguration(typeof(InstantMessageScheduleEntityConfiguration))]
public class InstantMessageSchedule : MessageSchedule
{
    public DateTime? SendDate { get; set; }
    public string? DispatchStatus { get; set; }
}