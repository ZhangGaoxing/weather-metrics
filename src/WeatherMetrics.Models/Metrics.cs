using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherMetrics.Models
{
    public class Metrics
    {
        [Column("time")]
        public DateTime Time { get; set; } = DateTime.Now;

        [Column("device_id")]
        public string DeviceId { get; set; }

        [Column("temperature")]
        public double Temperature { get; set; }

        [Column("humidity")]
        public double Humidity { get; set; }

        [Column("pressure")]
        public double Pressure { get; set; }

        [Column("image_base64")]
        public string ImageBase64 { get; set; }

        public static bool Insert(DbContext context, Metrics metrics)
        {
            int row = context.Database.ExecuteSqlRaw("INSERT INTO metrics VALUES ({0}, {1}, {2}, {3}, {4}, {5})", metrics.Time, metrics.DeviceId, metrics.Temperature, metrics.Humidity, metrics.Pressure, metrics.ImageBase64);

            return row > 0;
        }
    }
}