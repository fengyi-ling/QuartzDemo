using Quartz;
using QuartzDemo.Service;

namespace QuartzDemo.Jobs;

public class ClearRecordJob : IJob
{
    private readonly IClearRecordJobService _clearRecordJobService;
    private readonly IRetentionRepository _retentionRepository;

    public ClearRecordJob(IClearRecordJobService clearRecordJobService, IRetentionRepository retentionRepository)
    {
        _clearRecordJobService = clearRecordJobService;
        _retentionRepository = retentionRepository;
    }

    public Task Execute(IJobExecutionContext context)
    {
        // Use "!" just for demo, can not apply to production code
        var dataMap = context.MergedJobDataMap;
        var domain = dataMap.GetString("domain");
        var maxRetryAttempts = int.Parse(dataMap.GetString("maxRetryAttempts")!);
        _retentionRepository.UpdateMaxRetryAttemptsByDomain(domain!, maxRetryAttempts);
        _clearRecordJobService.Clear(domain!);
        return Task.CompletedTask;
    }
}