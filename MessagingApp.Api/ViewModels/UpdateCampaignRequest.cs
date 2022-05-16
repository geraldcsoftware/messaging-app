using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.ViewModels;

public class UpdateCampaignRequest
{
    [FromRoute] public string? Id { get; set; }
    public string? Name { get; set; }
}