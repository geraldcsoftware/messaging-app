using Humanizer;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Scheduling;

public class ImmediateMessageDispatcher : IScheduleTrigger
{
    private readonly IIdGenerator _idGenerator;
    private readonly AppDbContext _dbContext;

    public ImmediateMessageDispatcher(IIdGenerator idGenerator, AppDbContext dbContext)
    {
        _idGenerator = idGenerator;
        _dbContext = dbContext;
    }

    public async Task<MessageSchedule> CreateSchedule(ScheduleRequest req, MessageCampaign campaign)
    {
        ArgumentNullException.ThrowIfNull(req);
        ArgumentNullException.ThrowIfNull(campaign);
        if (req.Customers.Count == 0)
        {
            throw new ArgumentException("Cannot schedule campaign with no customers");
        }


        var customers = await _dbContext.Customers
                                        .Where(x => req.Customers.Contains(x.Id))
                                        .ToListAsync();

        var schedule = DoCreateSchedule(DateTime.UtcNow, campaign, customers);

        _dbContext.Schedules.Add(schedule);
        await _dbContext.SaveChangesAsync();

        // Todo: Dispatch event to send message immediately
        return schedule;
    }

    private MessageSchedule DoCreateSchedule(DateTime sendDate, MessageCampaign campaign,
                                             IReadOnlyCollection<Customer> customers)
    {
        var messageSchedule = new InstantMessageSchedule
        {
            SendDate = sendDate.ToUniversalTime(),
            DispatchStatus = MessageDispatchStatus.Pending.Humanize(),
            Campaign = campaign,
            Created = DateTime.UtcNow,
            ClientId = campaign.ClientId,
            ScheduleType = ScheduleTypes.Instant,
            Id = _idGenerator.NewId(),
            IsActive = true,
            TargetCustomers = customers.ToArray()
        };

        return messageSchedule;
    }
}