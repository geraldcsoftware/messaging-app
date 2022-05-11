using FastEndpoints;
using Mapster;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class DeleteClientEndpoint : EndpointWithoutRequest<ClientViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DeleteClientEndpoint> _logger;

    public DeleteClientEndpoint(AppDbContext dbContext, ILogger<DeleteClientEndpoint> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("clients/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("id");
        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (client is null)
        {
            _logger.LogWarning("Client with id <<< {ClientId} >>> not found", id);
            await SendNotFoundAsync(ct);
            return;
        }

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync(ct);

        await SendAsync(client.Adapt<ClientViewModel>(), cancellation: ct);
    }
}