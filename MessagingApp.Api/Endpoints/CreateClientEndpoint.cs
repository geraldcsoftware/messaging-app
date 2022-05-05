using FastEndpoints;
using Humanizer;
using Mapster;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;

namespace MessagingApp.Api.Endpoints;

public class CreateClientEndpoint : Endpoint<CreateClientRequest, ClientViewModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IIdGenerator _idGenerator;

    public CreateClientEndpoint(AppDbContext dbContext,
                                IIdGenerator idGenerator)
    {
        _dbContext = dbContext;
        _idGenerator = idGenerator;
    }
    
    public override void Configure()
    {
        Post("clients");
    }

    public override async Task HandleAsync(CreateClientRequest req, CancellationToken ct)
    {
        var client = new Client
        {
            Id = _idGenerator.NewId(),
            Name = req.Name.Titleize(),
            Email = req.Email?.ToLower(),
            Created = DateTime.UtcNow
        };
        
        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync(ct);
        
        await SendAsync(client.Adapt<ClientViewModel>(), cancellation: ct);
    }
}