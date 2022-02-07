using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Media;
using Quartz;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeatherMetrics.Models;

namespace WeatherMetrics.ConsoleApp
{
    public class MetricsJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                WeatherContext context = (WeatherContext)AppConfig.ServiceProvider.GetService(typeof(WeatherContext));

                var metrics = GetMetrics();

                Console.WriteLine($"[{metrics.Time.ToString("yyyy/MM/dd HH:mm:ss")}] Temperature:{metrics.Temperature} Humidity:{metrics.Humidity} Pressure:{metrics.Pressure}");
                Metrics.Insert(context, metrics);
            });
        }

        private Metrics GetMetrics()
        {
            Bme280 bme = (Bme280)AppConfig.ServiceProvider.GetService(typeof(Bme280));

            bme.SetPowerMode(Bmx280PowerMode.Normal);

            bme.PressureSampling = Sampling.UltraHighResolution;
            bme.TemperatureSampling = Sampling.UltraHighResolution;
            bme.HumiditySampling = Sampling.UltraHighResolution;

            bme.TryReadPressure(out UnitsNet.Pressure p);
            bme.TryReadTemperature(out UnitsNet.Temperature t);
            bme.TryReadHumidity(out UnitsNet.RelativeHumidity h);

            bme.SetPowerMode(Bmx280PowerMode.Sleep);

            return new Metrics
            {
                DeviceId = Dns.GetHostName(),
                Temperature = Math.Round(t.DegreesCelsius, 2),
                Humidity = Math.Round(h.Percent, 2),
                Pressure = Math.Round(p.Pascals, 2)
            };
        }

        private string GetImage()
        {
            VideoDevice video = (VideoDevice)AppConfig.ServiceProvider.GetService(typeof(VideoDevice));

            byte[] image = video.Capture();
            return Convert.ToBase64String(image);
        }
    }
}
