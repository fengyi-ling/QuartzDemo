namespace QuartzDemo.Client;

public interface IEventHubClient
{
    bool Publish(string topic, string message);
}