using FastEndpoints;
using Mapster;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

[HttpGet("clients/{id}"), Authorize]
public class GetClientByIdEndpoint : EndpointWithoutRequest<ClientViewModel>
{
    private readonly AppDbContext _dbContext;

    public GetClientByIdEndpoint(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override async Task HandleAsync(CancellationToken ct)
    {
        var clientId = Route<string>("id");

        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId, ct);

        if (client is null) await SendNotFoundAsync(ct);
        else await SendAsync(client.Adapt<ClientViewModel>(), cancellation: ct);
    }
}