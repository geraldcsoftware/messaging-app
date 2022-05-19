using FastEndpoints;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;

namespace MessagingApp.Api.Endpoints;

public class CreateCampaignEndpoint : EndpointWithMapping<CreateCampaignRequest, CampaignViewModel, MessageCampaign>
{
    private readonly AppDbContext _dbContext;
    private readonly IIdGenerator _idGenerator;
    private readonly ILogger<CreateCampaignEndpoint> _logger;

    public CreateCampaignEndpoint(AppDbContext dbContext,
                                  IIdGenerator idGenerator,
                                  ILogger<CreateCampaignEndpoint> logger)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("clients/{clientId}/campaigns");
    }

    public override async Task HandleAsync(CreateCampaignRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating campaign {@Campaign}", req);
        var entity = MapToEntity(req);

        _dbContext.Campaigns.Add(entity);

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Campaign saved {@Campaign}", entity);

        await SendCreatedAtAsync("GetCampaign", new { id = entity.Id }, MapFromEntity(entity), cancellation: ct);
    }

    public override MessageCampaign MapToEntity(CreateCampaignRequest r) => new()
    {
        Id = _idGenerator.NewId(),
        Name = r.Name,
        Template = r.MessageTemplate,
        ClientId = r.ClientId
    };

    public override CampaignViewModel MapFromEntity(MessageCampaign e)
    {
        return new()
        {
            Id = e.Id,
            Name = e.Name,
            MessageTemplate = e.Template,
            Created = DateTime.UtcNow,
            Active = false,
            ClientId = e.ClientId,
        };
    }
}