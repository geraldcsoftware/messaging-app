using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.ViewModels;

public class CreateCampaignRequest
{
    public string? Name { get; set; }
    [FromRoute(Name = "campaignId")] public string? ClientId { get; set; }
    public string? MessageTemplate { get; set; }
    public ICollection<string> PlaceHolders { get; set; } = new List<string>();
}