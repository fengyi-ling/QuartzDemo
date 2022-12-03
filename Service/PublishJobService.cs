using QuartzDemo.Client;
using QuartzDemo.Domain;
using QuartzDemo.Repository;

namespace QuartzDemo.Service;

public class PublishJobService : IPublishJobService
{
    private readonly IEventHubClient _eventHubClient;
    private readonly IReprocessRepository _repository;

    private readonly Dictionary<string, string> _domainToTopic = new()
    {
        { "apple", "apple-topic" },
        { "orange", "orange-topic" },
        { "banana", "banana-topic" }
    };


    public PublishJobService(IEventHubClient eventHubClient, IReprocessRepository repository)
    {
        _eventHubClient = eventHubClient;
        _repository = repository;
    }

    public void Publish(string domain)
    {
        var topic = _domainToTopic[domain];
        var reprocesses = _repository.FindAllByDomainAndStatus(domain, false);
        reprocesses.ToList()
            .ForEach(reprocess => PublishAndUpdateRepository(topic, reprocess));
    }

    private void PublishAndUpdateRepository(string topic, Reprocess reprocess)
    {
        var isPublishSuccessful = _eventHubClient.Publish(topic, reprocess.Message);
        if (isPublishSuccessful)
        {
            _repository.UpdateStatusById(reprocess.Id, true);
        }
        else
        {
            _repository.IncreaseOneRetryAttemptById(reprocess.Id);
        }
    }
}