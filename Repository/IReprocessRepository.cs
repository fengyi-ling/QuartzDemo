using QuartzDemo.Domain;

namespace QuartzDemo.Repository;

public interface IReprocessRepository
{
    void UpdateStatusById(int id, bool status);
    void IncreaseOneRetryAttemptById(int id);
    ICollection<Reprocess> FindAllByDomainAndStatus(string domain, bool status);
}