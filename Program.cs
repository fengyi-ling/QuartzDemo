using Quartz;
using QuartzDemo;

var builder = WebApplication.CreateBuilder(args);
// This is very important, quartz will not be about to read from appsettings without this line
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var helloJobKey = new JobKey("HelloProgram");
    q.AddJob<HelloJob>(opts => opts.WithIdentity(helloJobKey));
    q.AddTrigger(opts => opts
        .ForJob(helloJobKey)
        .WithIdentity("program-trigger")
        .WithCronSchedule("*/5 * * * * ?"));
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
var webApplication = builder.Build();

var schedulerFactory = webApplication.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.Start();

webApplication.UseRouting();
webApplication.Run();