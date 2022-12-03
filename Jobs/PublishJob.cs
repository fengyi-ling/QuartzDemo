using Quartz;

namespace QuartzDemo.Jobs;

public class PublishJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;                                                                                                                                                                                                                          
        var domain = dataMap.GetString("domain");
        var topic = dataMap.GetString("topic");
        
        return Console.Out.WriteAsync($"Select data: {domain}, Send to topic: {topic} \n\t");
    }
}