using Quartz;
using Quartz.Impl.Matchers;
using QuartzDemo.Dto;

namespace QuartzDemo.Service;

public class ClearRecordTriggerService : IClearRecordTriggerService
{
    private const string ClearRecordGroup = "clear-record";
    private readonly ISchedulerFactory _schedulerFactory;

    public ClearRecordTriggerService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task<DateTimeOffset> CreateTrigger(ClearRecordTriggerInfo triggerInfo)
    {
        var cronTrigger = TriggerBuilder.Create()
            .ForJob(new JobKey("clear-record", ClearRecordGroup))
            .WithIdentity(triggerInfo.Domain, ClearRecordGroup)
            .UsingJobData("domain", triggerInfo.Domain)
            .UsingJobData("maxRetryAttempts", triggerInfo.MaxRetryAttempts)
            .WithCronSchedule(triggerInfo.CronExpression)
            .Build();
        var scheduler = await _schedulerFactory.GetScheduler();
        return await scheduler.ScheduleJob(cronTrigger);
    }
    
    public async Task<TriggerKey> DeleteTrigger(string domain)
    {
        var triggerKey = new TriggerKey(domain, ClearRecordGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.UnscheduleJob(triggerKey);
        return triggerKey;
    }
    
    
    public async Task<DateTimeOffset?> UpdateTrigger(ClearRecordTriggerInfo triggerInfo)
    {
        var triggerKey = new TriggerKey(triggerInfo.Domain, ClearRecordGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        // Should check nullable original trigger in production code
        var newTrigger = originalTrigger!.GetTriggerBuilder()
            .UsingJobData("maxRetryAttempts", triggerInfo.MaxRetryAttempts)
            .WithCronSchedule(triggerInfo.CronExpression)
            .Build();
        return await scheduler.RescheduleJob(triggerKey, newTrigger);
    }
    
    public async Task<IReadOnlyCollection<TriggerKey>> GetAllTriggers()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(ClearRecordGroup));
        return triggerKeys;
    }
}