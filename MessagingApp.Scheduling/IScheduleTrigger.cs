using MessagingApp.Infrastructure.Models;

namespace MessagingApp.Scheduling;

public interface IScheduleTrigger
{
    Task<MessageSchedule> CreateSchedule(ScheduleRequest req, MessageCampaign campaign);
}