using WeatherMetrics.Models;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using System.Device.I2c;
using Iot.Device.Media;
using System.Net.NetworkInformation;
using System.Net;
using Quartz;
using WeatherMetrics.ConsoleApp;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;

using WeatherContext context = new WeatherContext();

I2cConnectionSettings i2cSettings = new I2cConnectionSettings(busId: 0, deviceAddress: Bmx280Base.SecondaryI2cAddress);
using I2cDevice i2c = I2cDevice.Create(i2cSettings);
using Bme280 bme = new Bme280(i2c);

VideoConnectionSettings videoSettings = new VideoConnectionSettings(busId: 0, captureSize: (640, 480));
using VideoDevice video = VideoDevice.Create(videoSettings);

AppConfig.ServiceProvider = new ServiceCollection()
            .AddSingleton(context)
            .AddSingleton(bme)
            .AddSingleton(video)
            .BuildServiceProvider();

// 创建一个触发器
var trigger = TriggerBuilder.Create()
    .WithCronSchedule("0 0/1 * * * ? *")
    .Build();
// 创建任务
var jobDetail = JobBuilder.Create<MetricsJob>()
    .WithIdentity("job", "group")
    .Build();
// 绑定调度器
ISchedulerFactory factory = new StdSchedulerFactory();
var scheduler = await factory.GetScheduler();
await scheduler.ScheduleJob(jobDetail, trigger);
await scheduler.Start();

Console.Read();
