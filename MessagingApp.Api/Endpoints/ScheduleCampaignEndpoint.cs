using FastEndpoints;
using FluentValidation.Results;
using MapsterMapper;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using MessagingApp.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Api.Endpoints;

public class ScheduleCampaignEndpoint : Endpoint<ScheduleCampaignRequest, CampaignViewModel>
{
    private readonly IScheduleTriggerFactory _scheduleTriggerFactory;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleCampaignEndpoint> _logger;

    public ScheduleCampaignEndpoint(IScheduleTriggerFactory scheduleTriggerFactory,
                                    AppDbContext dbContext,
                                    IMapper mapper,
                                    ILogger<ScheduleCampaignEndpoint> logger)
    {
        _scheduleTriggerFactory = scheduleTriggerFactory;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override void Configure()
    {
        base.Configure();
        Post("schedules");
        Claims("ClientId");
    }

    public override async Task HandleAsync(ScheduleCampaignRequest req, CancellationToken ct)
    {
        // get schedule type
        var scheduleTrigger = await _scheduleTriggerFactory.GetSchedule(req.ScheduleType!);

        if (scheduleTrigger is null)
        {
            _logger.LogWarning("Invalid schedule type '{ScheduleType}", req.ScheduleType);
            ValidationFailures.Add(new ValidationFailure(nameof(req.ScheduleType), "Invalid schedule type"));
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var campaign = await _dbContext.Campaigns.FirstOrDefaultAsync(c => c.Id == req.CampaignId, ct);

        if (campaign is null)
        {
            _logger.LogInformation("Campaign with Id '{CampaignId}' not found. Cannot create schedule", req.CampaignId);
            ValidationFailures.Add(new ValidationFailure(nameof(req.CampaignId), "Campaign not found"));
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var scheduleRequest = _mapper.Map<ScheduleRequest>(req);
        var schedule = await scheduleTrigger.CreateSchedule(scheduleRequest, campaign);
        _dbContext.Schedules.Add(schedule);

        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Schedule created successfully");

        await SendOkAsync(_mapper.Map<CampaignViewModel>(campaign), ct);
    }
}