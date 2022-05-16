using FastEndpoints;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Endpoints;

public class ListCampaignEndpoint : EndpointWithoutRequest<PagedCollection<CampaignViewModel>>
{
    public override void Configure()
    {
        Get("clients/{clientId}/campaigns");
    }
    public override Task HandleAsync(CancellationToken ct)
    {
        return base.HandleAsync(ct);
    }
}