using Quartz;
using QuartzDemo.Client;
using QuartzDemo.Jobs;
using QuartzDemo.Repository;
using QuartzDemo.Service;

var builder = WebApplication.CreateBuilder(args);
// This is very important, quartz will not be about to read from appsettings without this line
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    var publishJobKey = new JobKey("publish", "publish");
    q.AddJob<PublishJob>(opts => opts.WithIdentity(publishJobKey));
    
    var clearRecordJobKey = new JobKey("clear-record", "clear-record");
    q.AddJob<ClearRecordJob>(opts => opts.WithIdentity(clearRecordJobKey));
    
    q.AddTrigger(opts =>
    {
        const string domain = "orange";
        const string topic = "orange";
        opts
            .ForJob(publishJobKey)
            .WithIdentity(domain, "publish")
            .UsingJobData("domain", domain)
            .UsingJobData("topic", topic)
            .WithCronSchedule("*/50 * * * * ?");
    });
    
    q.AddTrigger(opts =>
    {
        opts
            .ForJob(clearRecordJobKey)
            .WithIdentity("orange", "clear-record")
            .UsingJobData("domain", "orange")
            .UsingJobData("maxRetryAttempts", "3")
            .WithCronSchedule("*/15 * * * * ?");
    });
    
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
builder.Services.AddControllers();

builder.Services.AddSingleton<IReprocessRepository, ReprocessRepository>();
builder.Services.AddSingleton<IEventHubClient, EventHubClient>();
builder.Services.AddSingleton<IPublishJobService, PublishJobService>();

var app = builder.Build();
var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.Start();


app.MapControllers();
app.Run();