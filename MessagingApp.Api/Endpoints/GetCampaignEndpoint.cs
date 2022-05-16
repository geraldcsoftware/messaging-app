using FastEndpoints;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Endpoints;

public class GetCampaignEndpoint : EndpointWithoutRequest<CampaignViewModel>
{
    public override void Configure()
    {
        Get("clients/{clientId}/campaigns/{campaignId}");
    }
    public override Task HandleAsync(CancellationToken ct)
    {
        return base.HandleAsync(ct);
    }
}