using Quartz;

namespace QuartzDemo.Jobs;

public class ClearRecordJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;                                                                                                                                                                                                                          
        var domain = dataMap.GetString("domain");
        var maxRetryAttempts = dataMap.GetString("maxRetryAttempts");
        
        return Console.Out.WriteAsync($"Delete data {domain} from table where RETRY_ATTEMPTS >= {maxRetryAttempts} \n\t");
    }
}