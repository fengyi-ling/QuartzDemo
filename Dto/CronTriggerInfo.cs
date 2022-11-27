namespace QuartzDemo.Dto;

public class CronTriggerInfo
{
    public CronTriggerInfo(int intervalOfSecond, string triggerName, string group)
    {
        IntervalOfSecond = intervalOfSecond;
        TriggerName = triggerName;
        Group = group;
    }

    public int IntervalOfSecond { get; }
    public string TriggerName { get; }
    public string Group { get; }
}