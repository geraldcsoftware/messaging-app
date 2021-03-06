namespace MessagingApp.Api.ViewModels;

public class ScheduleCampaignRequest
{
    public string? CampaignId { get; set; }
    public string? ClientId { get; set; }
    public string? ScheduleType { get; set; }
    public string? ScheduleValue { get; set; }
    public IReadOnlyCollection<string> Customers { get; } = new List<string>();
}