using Quartz;
using QuartzDemo;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddQuartz(q => { q.UseMicrosoftDependencyInjectionJobFactory(); });
        services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
    }).Build();
var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

var job = JobBuilder.Create<HelloJob>()
    .WithIdentity("myJob", "group1")
    .Build();

var trigger = TriggerBuilder.Create()
    .WithIdentity("myTrigger", "group1")
    .ForJob("myJob", "group1")
    .WithCronSchedule("*/5 * * * * ?")
    .Build();

await scheduler.ScheduleJob(job, trigger);

// will block until the last running job completes
await builder.RunAsync();