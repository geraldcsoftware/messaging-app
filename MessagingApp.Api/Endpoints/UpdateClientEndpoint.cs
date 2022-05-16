using FastEndpoints;
using Mapster;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class UpdateClientEndpoint : Endpoint<UpdateClientRequest, ClientViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UpdateClientEndpoint> _logger;

    public UpdateClientEndpoint(AppDbContext dbContext, ILogger<UpdateClientEndpoint> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("clients/{id}");
    }

    public override async Task HandleAsync(UpdateClientRequest req, CancellationToken ct)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (client is null)
        {
            _logger.LogWarning("Client with Id {Id} not found", req.Id);
            await SendNotFoundAsync(ct);
            return;
        }

        client.Name = req.Name;

        _logger.LogInformation("Updating client {@Client}", client);
        
        await _dbContext.SaveChangesAsync(ct);
        await SendAsync(client.Adapt<ClientViewModel>(), cancellation: ct);
    }
}