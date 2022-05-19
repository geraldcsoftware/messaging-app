using FastEndpoints;
using MapsterMapper;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class GetCampaignEndpoint : EndpointWithoutRequest<CampaignViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCampaignEndpoint> _logger;

    public GetCampaignEndpoint(AppDbContext dbContext,
                               IMapper mapper,
                               ILogger<GetCampaignEndpoint> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("clients/{clientId}/campaigns/{campaignId}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var clientId = Route<string>("clientId");
        var campaignId = Route<string>("campaignId");

        var campaign = await _dbContext.Campaigns
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(c => c.ClientId == clientId
                                                                 && c.Id    == campaignId, ct);

        if (campaign == null)
        {
            _logger.LogWarning("Campaign with id {CampaignId} & ClientId {ClientId} not found", campaignId, clientId);
            await SendNotFoundAsync(ct);
        }
        else await SendAsync(_mapper.Map<CampaignViewModel>(campaign), cancellation: ct);
    }
}