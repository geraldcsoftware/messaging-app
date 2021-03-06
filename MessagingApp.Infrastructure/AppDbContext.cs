using MessagingApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<MessageCampaign> Campaigns => Set<MessageCampaign>();
    public DbSet<MessageSchedule> Schedules => Set<MessageSchedule>();
    public DbSet<Customer> Customers => Set<Customer>();
}