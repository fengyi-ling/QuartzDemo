using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzDemo.Dto;

namespace QuartzDemo.Controller;

[ApiController]
[Route("/")]
public class QuartzController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzController(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrigger([FromBody] CronTriggerInfo cronTriggerInfo)
    {
        var jobDetail = JobBuilder.Create<HelloJob>()
            .WithIdentity("hello", "controller")
            .Build();
        var cronTrigger = TriggerBuilder.Create()
            .WithIdentity(cronTriggerInfo.TriggerName, cronTriggerInfo.Group)
            .WithCronSchedule($"*/{cronTriggerInfo.IntervalOfSecond} * * * * ?")
            .Build();
        var scheduler = await _schedulerFactory.GetScheduler();
        var dateTimeOffset = await scheduler.ScheduleJob(jobDetail, cronTrigger);
        return Ok("next fire time: " + dateTimeOffset);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateTrigger([FromBody] CronTriggerInfo cronTriggerInfo)
    {
        var triggerKey = new TriggerKey(cronTriggerInfo.TriggerName, cronTriggerInfo.Group);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        if (originalTrigger == null)
        {
            return NotFound();
        }

        var newTrigger = originalTrigger.GetTriggerBuilder()
            .WithCronSchedule($"*/{cronTriggerInfo.IntervalOfSecond} * * * * ?")
            .Build();
        var dateTimeOffset = await scheduler.RescheduleJob(triggerKey, newTrigger);
        return Ok("next fire time: " + dateTimeOffset);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteTrigger([FromBody] CronTriggerInfo cronTriggerInfo)
    {
        var triggerKey = new TriggerKey(cronTriggerInfo.TriggerName, cronTriggerInfo.Group);
        var scheduler = await _schedulerFactory.GetScheduler();
        var originalTrigger = await scheduler.GetTrigger(triggerKey);
        if (originalTrigger == null)
        {
            return NotFound();
        }

        await scheduler.UnscheduleJob(triggerKey);
        return Ok($"Successfully delete trigger {triggerKey}" );
    }
}