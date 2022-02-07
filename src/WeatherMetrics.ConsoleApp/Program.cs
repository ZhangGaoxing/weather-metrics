using Iot.Device.Bmxx80;
using Iot.Device.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System.Device.I2c;
using WeatherMetrics.ConsoleApp;
using WeatherMetrics.Models;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

using WeatherContext context = new WeatherContext(config["ConnectionString"]);

I2cConnectionSettings i2cSettings = new I2cConnectionSettings(busId: 0, deviceAddress: Bmx280Base.SecondaryI2cAddress);
using I2cDevice i2c = I2cDevice.Create(i2cSettings);
using Bme280 bme = new Bme280(i2c);

VideoConnectionSettings videoSettings = new VideoConnectionSettings(busId: 0, captureSize: (640, 480));
using VideoDevice video = VideoDevice.Create(videoSettings);

AppConfig.ServiceProvider = new ServiceCollection()
    .AddSingleton(config)
    .AddSingleton(context)
    .AddSingleton(bme)
    .AddSingleton(video)
    .BuildServiceProvider();

var trigger = TriggerBuilder.Create()
    .WithCronSchedule(config["QuartzCron"])
    .Build();

var jobDetail = JobBuilder.Create<MetricsJob>()
    .WithIdentity("job", "group")
    .Build();

ISchedulerFactory factory = new StdSchedulerFactory();
var scheduler = await factory.GetScheduler();
await scheduler.ScheduleJob(jobDetail, trigger);
await scheduler.Start();

Console.Read();
