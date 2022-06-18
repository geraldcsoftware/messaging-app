using Microsoft.Extensions.DependencyInjection;

namespace MessagingApp.Scheduling;

public class ScheduleTriggerFactory : IScheduleTriggerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ScheduleTriggerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<IScheduleTrigger?> GetSchedule(string key)
    {
        var schedule = key switch
        {
            ScheduleTypes.Instant => ActivatorUtilities.CreateInstance<ImmediateMessageDispatcher>(_serviceProvider),
            _                     => null
        };

        return Task.FromResult<IScheduleTrigger?>(schedule);
    }
}