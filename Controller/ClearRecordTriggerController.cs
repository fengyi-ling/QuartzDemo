using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzDemo.Dto;

namespace QuartzDemo.Controller;

[ApiController]
[Route("triggers/clear-record")]
public class ClearRecordTriggerController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;
    private const string ClearRecordGroup = "clear-record";

    public ClearRecordTriggerController(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrigger([FromBody] ClearRecordTriggerInfo triggerInfo)
    {
        var cronTrigger = TriggerBuilder.Create()
            .ForJob(new JobKey("clear-record", ClearRecordGroup))
            .WithIdentity(triggerInfo.Domain, ClearRecordGroup)
            .UsingJobData("domain", triggerInfo.Domain)
            .UsingJobData("maxRetryAttempts", triggerInfo.MaxRetryAttempts)
            .WithCronSchedule(triggerInfo.CronExpression)
            .Build();
        var scheduler = await _schedulerFactory.GetScheduler();
        var dateTimeOffset = await scheduler.ScheduleJob(cronTrigger);
        return Ok("next fire time: " + dateTimeOffset);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateTrigger([FromBody] ClearRecordTriggerInfo triggerInfo)
    {
        var triggerKey = new TriggerKey(triggerInfo.Domain, ClearRecordGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        if (originalTrigger == null)
        {
            return NotFound();
        }

        var newTrigger = originalTrigger.GetTriggerBuilder()
            .UsingJobData("maxRetryAttempts", triggerInfo.MaxRetryAttempts)
            .WithCronSchedule(triggerInfo.CronExpression)
            .Build();
        var dateTimeOffset = await scheduler.RescheduleJob(triggerKey, newTrigger);
        return Ok("next fire time: " + dateTimeOffset);
    }

    [HttpDelete("{domain}")]
    public async Task<IActionResult> DeleteTrigger([FromRoute] string domain)
    {
        var triggerKey = new TriggerKey(domain, ClearRecordGroup);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        if (originalTrigger == null)
        {
            return NotFound();
        }

        await scheduler.UnscheduleJob(triggerKey);
        return Ok($"Successfully delete trigger {triggerKey}");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTriggers()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(ClearRecordGroup));
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(ClearRecordGroup));
        return Ok($"All jobs:\n{string.Join("\n\t", jobKeys)}\n" +
                  $"All triggers:\n{string.Join("\n", triggerKeys)}");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllTriggersJustForTest()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(ClearRecordGroup));
        await scheduler.UnscheduleJobs(triggerKeys);
        return Ok($"Successfully delete trigger {string.Join(",", triggerKeys)}");
    }
}