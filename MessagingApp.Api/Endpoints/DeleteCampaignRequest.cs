using FastEndpoints;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Endpoints;

public class DeleteCampaignRequest : EndpointWithoutRequest<CampaignViewModel>
{
    public override void Configure()
    {
        Delete("clients/{clientId}/campaigns/{campaignId}");
    }

    public override Task<CampaignViewModel> ExecuteAsync(CancellationToken ct)
    {
        return base.ExecuteAsync(ct);
    }
}