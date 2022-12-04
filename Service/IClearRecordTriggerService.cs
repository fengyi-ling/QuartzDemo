using Quartz;
using QuartzDemo.Dto;

namespace QuartzDemo.Service;

public interface IClearRecordTriggerService
{
    Task<DateTimeOffset> CreateTrigger(ClearRecordTriggerInfo triggerInfo);
    Task<TriggerKey> DeleteTrigger(string domain);
    Task<DateTimeOffset?> UpdateTrigger(ClearRecordTriggerInfo triggerInfo);
    Task<IReadOnlyCollection<TriggerKey>> GetAllTriggers();
}