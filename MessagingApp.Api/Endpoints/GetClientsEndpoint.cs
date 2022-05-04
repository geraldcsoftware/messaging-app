using FastEndpoints;
using MessagingApp.Api.Configuration;
using MessagingApp.Api.ViewModels;
using Microsoft.Extensions.Options;

namespace MessagingApp.Api.Endpoints;

public class GetClientsEndpoint : EndpointWithoutRequest<PagedCollection<ClientViewModel>>
{
    private readonly IOptions<PagingOptions> _pagingOptions;

    public GetClientsEndpoint(IOptions<PagingOptions> pagingOptions)
    {
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

        await Task.Delay(01, ct);
        return new PagedCollection<ClientViewModel>
        {
            Page = page,
            PageSize = pageSize,
            Items = Array.Empty<ClientViewModel>(),
            TotalItems = 0,
            TotalPages = 0
        };
    }
}