namespace MessagingApp.Scheduling;

public interface IScheduleTriggerFactory
{
    public Task<IScheduleTrigger?> GetSchedule(string key);
}