using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMetrics.ConsoleApp
{
    internal static class AppConfig
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
