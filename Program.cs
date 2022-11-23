using Quartz;
using QuartzDemo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var helloJobKey = new JobKey("Hello");
    q.AddJob<HelloJob>(opts => opts.WithIdentity(helloJobKey));
    q.AddTrigger(opts => opts
        .ForJob(helloJobKey)
        .WithIdentity("hello-trigger")
        .WithCronSchedule("*/5 * * * * ?"));
    q.SchedulerId = "custom-schedule";
    q.SetProperty("quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz");
    q.SetProperty("quartz.serializer.type", "json");
    q.SetProperty("quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz");
    q.SetProperty("quartz.jobStore.tablePrefix", "QRTZ_");
    q.SetProperty("quartz.jobStore.dataSource", "myDS");
    q.SetProperty("quartz.dataSource.myDS.connectionString",
        "Server=localhost;Database=quartznet;User=fengyi;Pwd=placeholder");
    q.SetProperty("quartz.dataSource.myDS.provider", "MySql");
});
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
var webApplication = builder.Build();

var schedulerFactory = webApplication.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

await scheduler.Start();
await webApplication.RunAsync();