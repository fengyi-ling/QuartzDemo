using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzDemo.Dto;

namespace QuartzDemo.Controller
{
    [ApiController]
    [Route("triggers")]
    public class TriggerController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        private readonly Dictionary<string, string> _domainToTopic = new()
        {
            { "apple", "service.bus.apple" },
            { "orange", "service.bus.orange" },
            { "banana", "service.bus.banana" },
        };


        public TriggerController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var cronTrigger = TriggerBuilder.Create()
                .ForJob(new JobKey("publish", "program"))
                .WithIdentity(publishTriggerInfo.Domain, publishTriggerInfo.Domain)
                .UsingJobData("domain", publishTriggerInfo.Domain)
                .UsingJobData("topic", _domainToTopic[publishTriggerInfo.Domain])
                .WithCronSchedule(publishTriggerInfo.CronExpression)
                .Build();
            var scheduler = await _schedulerFactory.GetScheduler();
            var dateTimeOffset = await scheduler.ScheduleJob(cronTrigger);
            return Ok("next fire time: " + dateTimeOffset);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var triggerKey = new TriggerKey(publishTriggerInfo.Domain, publishTriggerInfo.Domain);
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
            var triggerKey = new TriggerKey(domain, domain);
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
        public async Task<IActionResult> GetTriggers()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return Ok($"All jobs:\n{string.Join("\n\t", jobKeys)}\n" +
                      $"All triggers:\n{string.Join("\n", triggerKeys)}");
        }
    
        [HttpDelete]
        public async Task<IActionResult> DeleteAllTriggersJustForTest()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var originalTrigger = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            await scheduler.UnscheduleJobs(originalTrigger);
            return Ok($"Successfully delete trigger {string.Join(",", originalTrigger)}");
        }
    }
}