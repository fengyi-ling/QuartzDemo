namespace QuartzDemo.Domain;

public class Retention
{
    public Retention(string domain, int maxRetryAttempts)
    {
        MaxRetryAttempts = maxRetryAttempts;
        Domain = domain;
    }

    public int MaxRetryAttempts { get; set; }
    public string Domain { get; set; }
}