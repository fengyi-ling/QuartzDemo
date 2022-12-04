using Microsoft.AspNetCore.Mvc;
using QuartzDemo.Dto;
using QuartzDemo.Service;

namespace QuartzDemo.Controller
{
    [ApiController]
    [Route("triggers/publish")]
    public class PublishTriggerController : ControllerBase
    {
        private readonly IPublishTriggerService _publishTriggerService;
        
        public PublishTriggerController(IPublishTriggerService publishTriggerService)
        {
            _publishTriggerService = publishTriggerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var dateTimeOffset = await _publishTriggerService.CreateTrigger(publishTriggerInfo);
            return Ok("next fire time: " + dateTimeOffset);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTrigger([FromBody] PublishTriggerInfo publishTriggerInfo)
        {
            var dateTimeOffset = await _publishTriggerService.UpdateTrigger(publishTriggerInfo);
            return Ok("next fire time: " + dateTimeOffset);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTriggers()
        {
            var triggerKeys = await _publishTriggerService.GetAllTriggers();
            return Ok($"All triggers:\n{string.Join("\n", triggerKeys)}");
        }
        
        [HttpDelete("{domain}")]
        public async Task<IActionResult> DeleteTrigger([FromRoute] string domain)
        {
            var triggerKey = await _publishTriggerService.DeleteTrigger(domain);
            return Ok($"Successfully delete trigger {triggerKey}");
        }
    }
}