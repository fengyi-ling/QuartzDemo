using Quartz;
using QuartzDemo;
using QuartzDemo.Jobs;

var builder = WebApplication.CreateBuilder(args);
// This is very important, quartz will not be about to read from appsettings without this line
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var publishJobKey = new JobKey("publish", "program");
    q.AddJob<PublishJob>(opts => opts.WithIdentity(publishJobKey));
    q.AddTrigger(opts =>
    {
        const string domain = "static-domain";
        const string topic = "static-topic";
        const string cronExpression = "*/30 * * * * ?";
        opts
            .ForJob(publishJobKey)
            .WithIdentity(domain, topic)
            .UsingJobData("domain", domain)
            .UsingJobData("topic", topic)
            .WithCronSchedule(cronExpression);
    });
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
builder.Services.AddControllers();

var app = builder.Build();
var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.Start();


app.MapControllers();
app.Run();