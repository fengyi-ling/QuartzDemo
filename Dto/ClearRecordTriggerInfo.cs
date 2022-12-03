namespace QuartzDemo.Dto;

public class ClearRecordTriggerInfo
{
    public string Domain { get; }
    public string MaxRetryAttempts { get; }
    public string CronExpression { get; }

    public ClearRecordTriggerInfo(string domain, string maxRetryAttempts, string cronExpression)
    {
        Domain = domain;
        MaxRetryAttempts = maxRetryAttempts;
        CronExpression = cronExpression;
    }
}