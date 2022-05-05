using FastEndpoints;
using Mapster;
using MessagingApp.Api.Configuration;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MessagingApp.Api.Endpoints;

public class GetClientsEndpoint : EndpointWithoutRequest<PagedCollection<ClientViewModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IOptions<PagingOptions> _pagingOptions;

    public GetClientsEndpoint(AppDbContext dbContext, IOptions<PagingOptions> pagingOptions)
    {
        _dbContext = dbContext;
        _pagingOptions = pagingOptions;
    }

    public override void Configure()
    {
        Get("clients");
        AllowAnonymous();
    }

    public override async Task<PagedCollection<ClientViewModel>> ExecuteAsync(CancellationToken ct)
    {
        var page = Query<int?>("page", false)         ?? 1;
        var pageSize = Query<int?>("pageSize", false) ?? _pagingOptions.Value.DefaultPageSize;

        var count = await _dbContext.Clients.CountAsync(ct);
        var items = await _dbContext.Clients
                                    .AsNoTracking()
                                    .OrderBy(c => c.Name)
                                    .ThenBy(c => c.Created)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ProjectToType<ClientViewModel>()
                                    .ToListAsync(ct);

        return new()
        {
            Page = page,
            PageSize = pageSize,
            Items = items,
            TotalItems = count,
            TotalPages = count == 0 ? 0 : (int)Math.Ceiling((double)count / pageSize)
        };
    }
}