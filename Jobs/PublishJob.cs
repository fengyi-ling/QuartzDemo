using Quartz;
using QuartzDemo.Service;

namespace QuartzDemo.Jobs;

public class PublishJob : IJob
{
    private readonly IPublishJobService _publishJobService;

    public PublishJob(IPublishJobService publishJobService)
    {
        _publishJobService = publishJobService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var domain = dataMap.GetString("domain");
        _publishJobService.Excecute(domain);
        return Task.CompletedTask;
    }
}