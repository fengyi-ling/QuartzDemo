using Microsoft.AspNetCore.Mvc;
using QuartzDemo.Dto;
using QuartzDemo.Service;

namespace QuartzDemo.Controller;

[ApiController]
[Route("triggers/clear-record")]
public class ClearRecordTriggerController : ControllerBase
{
    private readonly IClearRecordTriggerService _clearRecordTriggerService;

    public ClearRecordTriggerController(IClearRecordTriggerService clearRecordTriggerService)
    {
        _clearRecordTriggerService = clearRecordTriggerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrigger([FromBody] ClearRecordTriggerInfo triggerInfo)
    {
        var dateTimeOffset = await _clearRecordTriggerService.CreateTrigger(triggerInfo);
        return Ok("Create trigger successfully, next fire time: " + dateTimeOffset);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateTrigger([FromBody] ClearRecordTriggerInfo triggerInfo)
    {
        var dateTimeOffset = await _clearRecordTriggerService.UpdateTrigger(triggerInfo);
        return Ok("next fire time: " + dateTimeOffset);
    }

    [HttpDelete("{domain}")]
    public async Task<IActionResult> DeleteTrigger([FromRoute] string domain)
    {
        var triggerKey = await _clearRecordTriggerService.DeleteTrigger(domain);
        return Ok($"Successfully delete trigger {triggerKey}");
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAllTriggers()
    {
        var triggerKeys = await _clearRecordTriggerService.GetAllTriggers();
        return Ok($"All triggers:\n{string.Join("\n", triggerKeys)}");
    }
}