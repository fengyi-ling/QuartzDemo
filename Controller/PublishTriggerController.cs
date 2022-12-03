using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzDemo.Dto;

namespace QuartzDemo.Controller
{
    [ApiController]
    [Route("triggers/publish")]
    public class PublishTriggerController : ControllerBase
    {
        private const string PublishGroup = "publish";
        private readonly ISchedulerFactory _schedulerFactory;
        
        public PublishTriggerController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var cronTrigger = TriggerBuilder.Create()
                .ForJob(new JobKey(PublishGroup, PublishGroup))
                .WithIdentity(publishTriggerInfo.Domain, PublishGroup)
                .UsingJobData("domain", publishTriggerInfo.Domain)
                .WithCronSchedule(publishTriggerInfo.CronExpression)
                .Build();
            var scheduler = await _schedulerFactory.GetScheduler();
            var dateTimeOffset = await scheduler.ScheduleJob(cronTrigger);
            return Ok("next fire time: " + dateTimeOffset);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var triggerKey = new TriggerKey(publishTriggerInfo.Domain, PublishGroup);
            var scheduler = await _schedulerFactory.GetScheduler();
            var originalTrigger = await scheduler.GetTrigger(triggerKey);
            if (originalTrigger == null)
            {
                return NotFound();
            }

            var newTrigger = originalTrigger.GetTriggerBuilder()
                .WithCronSchedule(publishTriggerInfo.CronExpression)
                .Build();
            var dateTimeOffset = await scheduler.RescheduleJob(triggerKey, newTrigger);
            return Ok("next fire time: " + dateTimeOffset);
        }

        [HttpDelete("{domain}")]
        public async Task<IActionResult> DeleteTrigger([FromRoute] string domain)
        {
            var triggerKey = new TriggerKey(domain, PublishGroup);
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
            var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(PublishGroup));
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return Ok($"All jobs:\n{string.Join("\n\t", jobKeys)}\n" +
                      $"All triggers:\n{string.Join("\n", triggerKeys)}");
        }
    
        [HttpDelete]
        public async Task<IActionResult> DeleteAllTriggersJustForTest()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(PublishGroup));
            await scheduler.UnscheduleJobs(triggerKeys);
            return Ok($"Successfully delete trigger {string.Join(",", triggerKeys)}");
        }
    }
}