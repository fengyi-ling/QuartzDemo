using Quartz;
using QuartzDemo.Dto;

namespace QuartzDemo.Service;

public interface IPublishTriggerService
{
    Task<TriggerKey> DeleteTrigger(string domain);
    Task<IReadOnlyCollection<TriggerKey>> GetAllTriggers();
    Task<DateTimeOffset?> UpdateTrigger(PublishTriggerInfo publishTriggerInfo);
    Task<DateTimeOffset> CreateTrigger(PublishTriggerInfo publishTriggerInfo);
}