using FastEndpoints;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Endpoints;

public class UpdateCampaignRequest : Endpoint<UpdateCampaignRequest, CampaignViewModel>
{
    public override void Configure()
    {
        Put("clients/{clientId}/campaigns");
    }

    public override Task HandleAsync(UpdateCampaignRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}