using QuartzDemo.Domain;

namespace QuartzDemo.Service;

public interface IRetentionRepository 
{
    Retention FindByDomain(string domain);
    void UpdateMaxRetryAttemptsByDomain(string domain, int maxRetryAttempts);
}