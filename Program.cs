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
    q.AddJob<PublishJob>(opts => opts.WithIdentity(publishJobKey).StoreDurably());
    
    var clearRecordJobKey = new JobKey("clear-record", "clear-record");
    q.AddJob<ClearRecordJob>(opts => opts.WithIdentity(clearRecordJobKey).StoreDurably());
    
    // Add initial trigger if necessary
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
builder.Services.AddControllers();

builder.Services.AddSingleton<IReprocessRepository, ReprocessRepository>();
builder.Services.AddSingleton<IEventHubClient, EventHubClient>();
builder.Services.AddSingleton<IPublishJobService, PublishJobService>();
builder.Services.AddSingleton<IClearRecordJobService, ClearRecordJobService>();
builder.Services.AddSingleton<IRetentionRepository, RetentionRepository>();
builder.Services.AddSingleton<IClearRecordTriggerService, ClearRecordTriggerService>();
builder.Services.AddSingleton<IPublishTriggerService, PublishTriggerService>();

var app = builder.Build();
var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.Start();

app.MapControllers();
app.Run();