using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IoTSuite.Shared
{
    public class WeatherForecast
    {
        [Key]
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public WeatherForecastSummaries Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class WeatherForecastDTO
    {
        public int TemperatureC { get; set; }

        public WeatherForecastSummaries Summary { get; set; }
    }

    public enum WeatherForecastSummaries
    {
        Freezing,
        Bracing,
        Chilly,
        Cool,
        Mild,
        Warm,
        Balmy,
        Hot,
        Sweltering,
        Scorching,
    }
}