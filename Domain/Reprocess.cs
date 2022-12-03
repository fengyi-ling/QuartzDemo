namespace QuartzDemo.Domain;

public class Reprocess
{
    public string Message { get; }
    public int Id { get; }
    public int RetryAttempt { get; set; }
    public bool Status { get; set; }
    public string Domain { get; }

    public Reprocess(int id, string message, int retryAttempt, bool status, string domain)
    {
        Message = message;
        RetryAttempt = retryAttempt;
        Status = status;
        Domain = domain;
        Id = id;
    }
}