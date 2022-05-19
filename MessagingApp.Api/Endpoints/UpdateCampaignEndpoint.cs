using FastEndpoints;
using MapsterMapper;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class UpdateCampaignEndpoint : Endpoint<UpdateCampaignRequest, CampaignViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCampaignEndpoint> _logger;

    public UpdateCampaignEndpoint(AppDbContext dbContext,
                                  IMapper mapper,
                                  ILogger<UpdateCampaignEndpoint> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("clients/{clientId}/campaigns/{id}");
    }

    public override async Task HandleAsync(UpdateCampaignRequest req, CancellationToken ct)
    {
        var campaign = await _dbContext.Campaigns.FirstOrDefaultAsync(c => c.Id == req.Id, cancellationToken: ct);

        if (campaign is null)
        {
            _logger.LogWarning("Update campaign failed. Campaign not found. Request -> {@Request}", req);
            await SendNotFoundAsync(ct);
            return;
        }

        campaign.Name = req.Name;
        campaign.Template = req.Template;

        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Campaign updated. {@Campaign}", campaign);

        await SendOkAsync(_mapper.Map<CampaignViewModel>(campaign), ct);
    }
}