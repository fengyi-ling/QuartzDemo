using Quartz;

namespace QuartzDemo;

public class HelloJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Console.Out.WriteAsync("Hello Quartz");
    }
}