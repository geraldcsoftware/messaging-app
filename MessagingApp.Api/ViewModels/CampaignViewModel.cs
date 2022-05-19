namespace MessagingApp.Api.ViewModels;

public class CampaignViewModel
{
    public string? Id { get; set; }
    public DateTime Created { get; set; }
    public string? ClientId { get; set; }
    public string? Name { get; set; }
    public string? MessageTemplate { get; set; }
    public bool Active { get; set; }
    public DateTime? LastRun { get; set; }
    public DateTime? NextRun { get; set; }
    public IReadOnlyCollection<string>? Triggers { get; set; }
}