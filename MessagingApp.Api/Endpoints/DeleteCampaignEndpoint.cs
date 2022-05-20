using FastEndpoints;
using MapsterMapper;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class DeleteCampaignEndpoint : EndpointWithoutRequest<CampaignViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteCampaignEndpoint> _logger;

    public DeleteCampaignEndpoint(AppDbContext dbContext,
                                  IMapper mapper,
                                  ILogger<DeleteCampaignEndpoint> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("clients/{clientId}/campaigns/{campaignId}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var campaignId = Route<string>("campaignId");
        var clientId = Route<string>("clientId");

        var campaign = await _dbContext.Campaigns
                                       .FirstOrDefaultAsync(c => c.ClientId == clientId &&
                                                                 c.Id       == campaignId, ct);

        if (campaign is null)
        {
            _logger.LogWarning("Campaign with id {Id} and ClientId {ClientId} not found",
                               campaignId,
                               clientId);

            await SendNotFoundAsync(ct);
            return;
        }

        _dbContext.Campaigns.Remove(campaign);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Campaign {@Campaign} deleted from database", campaign);
        await SendOkAsync(_mapper.Map<CampaignViewModel>(campaign), ct);
    }
}