using QuartzDemo.Repository;

namespace QuartzDemo.Service;

public class ClearRecordJobService : IClearRecordJobService
{
    private readonly IReprocessRepository _reprocessRepository;
    private readonly IRetentionRepository _retentionRepository;

    public ClearRecordJobService(IReprocessRepository reprocessRepository, IRetentionRepository retentionRepository)
    {
        _reprocessRepository = reprocessRepository;
        _retentionRepository = retentionRepository;
    }

    public void Clear(string domain)
    {
        var maxAttempts = _retentionRepository.FindByDomain(domain).MaxRetryAttempts;
        _reprocessRepository.FindAllByDomain(domain)
            .Where(reprocess => reprocess.RetryAttempt >= maxAttempts)
            .ToList()
            .ForEach(reprocess => _reprocessRepository.DeleteById(reprocess.Id));
        var findAllByDomain = _reprocessRepository.FindAllByDomain(domain);
        Console.Out.Write("Updated retention table:" + string.Join(",", findAllByDomain));
    }
}