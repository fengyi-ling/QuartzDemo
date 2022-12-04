using Quartz;
using Quartz.Impl.Matchers;
using QuartzDemo.Dto;

namespace QuartzDemo.Service;

public class PublishTriggerService : IPublishTriggerService
{
    private const string PublishGroup = "publish";
    private readonly ISchedulerFactory _schedulerFactory;

    public PublishTriggerService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task<TriggerKey> DeleteTrigger(string domain)
    {
        var triggerKey = new TriggerKey(domain, PublishGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.UnscheduleJob(triggerKey);
        return triggerKey;
    }
        
    public async Task<IReadOnlyCollection<TriggerKey>> GetAllTriggers()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        return await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(PublishGroup));
    }


    public async Task<DateTimeOffset?> UpdateTrigger(PublishTriggerInfo publishTriggerInfo)
    {
        var triggerKey = new TriggerKey(publishTriggerInfo.Domain, PublishGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        var newTrigger = originalTrigger!.GetTriggerBuilder()
            .WithCronSchedule(publishTriggerInfo.CronExpression)
            .Build();
        return await scheduler.RescheduleJob(triggerKey, newTrigger);
    }

    public async Task<DateTimeOffset> CreateTrigger(PublishTriggerInfo publishTriggerInfo)
    {
        var cronTrigger = TriggerBuilder.Create()
            .ForJob(new JobKey(PublishGroup, PublishGroup))
            .WithIdentity(publishTriggerInfo.Domain, PublishGroup)
            .UsingJobData("domain", publishTriggerInfo.Domain)
            .WithCronSchedule(publishTriggerInfo.CronExpression)
            .Build();
        var scheduler = await _schedulerFactory.GetScheduler();
        return await scheduler.ScheduleJob(cronTrigger);
    }
}