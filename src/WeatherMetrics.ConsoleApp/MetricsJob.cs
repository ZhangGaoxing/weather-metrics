using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Media;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using System.Net;
using WeatherMetrics.Models;

namespace WeatherMetrics.ConsoleApp
{
    public class MetricsJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                var metrics = GetMetrics();
                if (DateTime.Now.Minute == 0 || string.IsNullOrEmpty(AppConfig.WeatherType))
                {
                    AppConfig.WeatherType = await GetXinzhiWeatherAsync();
                }
                metrics.WeatherType = AppConfig.WeatherType;
                metrics.ImageBase64 = GetImage();

                WeatherContext context = (WeatherContext)AppConfig.ServiceProvider.GetService(typeof(WeatherContext));

                Console.WriteLine($"[{metrics.Time.ToString("yyyy/MM/dd HH:mm:ss")}] {metrics.WeatherType} Temperature:{metrics.Temperature} Humidity:{metrics.Humidity} Pressure:{metrics.Pressure}");
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

        private async Task<string> GetXinzhiWeatherAsync()
        {
            IConfigurationRoot config = (IConfigurationRoot)AppConfig.ServiceProvider.GetService(typeof(IConfigurationRoot));

            using HttpClient client = new HttpClient();

            try
            {
                var json = await client.GetStringAsync($"https://api.seniverse.com/v3/weather/now.json?key={config["Xinzhi:Key"]}&location={config["Xinzhi:Location"]}&language=zh-Hans&unit=c");
                return (string)JsonConvert.DeserializeObject<dynamic>(json).results[0].now.text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
