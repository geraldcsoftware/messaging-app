namespace MessagingApp.Infrastructure.Models;

public class MessageSchedule
{
    public string? Id { get; set; }
    public string? ScheduleType { get; set; }
    public DateTime Created { get; set; }
    public bool IsActive { get; set; }
    public string? ClientId { get; set; }
    public string? CampaignId { get; set; }
    public MessageCampaign? Campaign { get; set; }
    public Client? Client { get; set; }
    public ICollection<Customer> TargetCustomers { get; set; } = new List<Customer>();
}