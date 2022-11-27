using Quartz;
using QuartzDemo;

var builder = WebApplication.CreateBuilder(args);
// This is very important, quartz will not be about to read from appsettings without this line
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var helloJobKey = new JobKey("hello", "program");
    q.AddJob<HelloJob>(opts => opts.WithIdentity(helloJobKey));
    q.AddTrigger(opts => opts
        .ForJob(helloJobKey)
        .WithIdentity("program-trigger", "program")
        .WithCronSchedule("*/30 * * * * ?"));
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
builder.Services.AddControllers();

var app = builder.Build();
var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.Start();


app.MapControllers();
app.Run();