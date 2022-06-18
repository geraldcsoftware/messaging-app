using MessagingApp.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Models;

[EntityTypeConfiguration(typeof(CustomerEntityTypeConfiguration))]
public class Customer
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Msisdn { get; set; }
    public DateTime Created { get; set; }

    public ICollection<MessageSchedule> MessageSchedules { get; set; } = new List<MessageSchedule>();
}