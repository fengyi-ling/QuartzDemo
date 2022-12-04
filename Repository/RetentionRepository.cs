using QuartzDemo.Domain;
using QuartzDemo.Service;

namespace QuartzDemo.Repository;

public class RetentionRepository : IRetentionRepository
{
    private readonly HashSet<Retention> _retentions = new()
    {
        new Retention("apple", 3),
        new Retention("orange", 2),
        new Retention("banana", 5)
    };

    public Retention FindByDomain(string domain)
    {
        return _retentions.First(retention => retention.Domain == domain);
    }

    public void UpdateMaxRetryAttemptsByDomain(string domain, int maxRetryAttempts)
    {
        var retention = _retentions.First(retention => retention.Domain == domain);
        retention.MaxRetryAttempts = maxRetryAttempts;
        Console.Out.Write("Updated retention table:" + string.Join(",", _retentions));
    }
}