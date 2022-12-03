namespace QuartzDemo.Client;

public class EventHubClient : IEventHubClient
{
    public bool Publish(string topic, string message)
    {
        Console.Out.Write($"Start publishing message: {message} to topic:{topic}");

        if (message.Length % 2 == 0)
        {
            Console.Out.Write($"Failed to published message: {message} to topic:{topic}");
            return false;
        }

        Console.Out.Write($"Successful published message: {message} to topic:{topic}");
        return true;
    }
}