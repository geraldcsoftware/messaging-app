using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.ViewModels;

public class UpdateCampaignRequest
{
    [FromRoute(Name = "id")] public string? Id { get; set; }
    [FromRoute(Name = "clientId")] public string? ClientId { get; set; }
    public string? Name { get; set; }
    public string? Template { get; set; }
}