namespace QuartzDemo.Dto;

public class PublishTriggerInfo
{
    public PublishTriggerInfo(string cronExpression, string domain)
    {
        CronExpression = cronExpression;
        Domain = domain;
    }

    public string CronExpression { get; }
    public string Domain { get; }
}