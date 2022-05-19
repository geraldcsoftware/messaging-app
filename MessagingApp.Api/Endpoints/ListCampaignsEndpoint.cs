using FastEndpoints;
using MapsterMapper;
using MessagingApp.Api.Configuration;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MessagingApp.Api.Endpoints;

public class ListCampaignsEndpoint : EndpointWithoutRequest<PagedCollection<CampaignViewModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ListCampaignsEndpoint> _logger;
    private readonly IOptions<PagingOptions> _pagingOptions;

    public ListCampaignsEndpoint(AppDbContext dbContext,
                                 IMapper mapper,
                                 ILogger<ListCampaignsEndpoint> logger,
                                 IOptions<PagingOptions> pagingOptions)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _pagingOptions = pagingOptions;
    }

    public override void Configure()
    {
        Get("clients/{clientId}/campaigns");
    }

    public override async Task<PagedCollection<CampaignViewModel>> ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var clientId = Route<string>("clientId");

            var page = Query<int?>("page", false)         ?? 1;
            var pageSize = Query<int?>("pageSize", false) ?? _pagingOptions.Value.DefaultPageSize;

            var query = _dbContext.Campaigns.Where(c => c.ClientId == clientId);

            var count = await query.CountAsync(ct);
            var totalPages = count == 0 ? 0 : (int)Math.Ceiling((double)count / pageSize);
            var items = await query.AsNoTracking()
                                   .OrderBy(c => c.Created)
                                   .ThenBy(c => c.Name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(ct);

            return new()
            {
                Page = page,
                PageSize = pageSize,
                Items = _mapper.Map<List<CampaignViewModel>>(items),
                TotalItems = count,
                TotalPages = totalPages
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting campaigns");
            throw;
        }
    }
}