using System.Text.Json.Serialization;

namespace weatherservice.Models
{
    /// <summary>
    /// Represents the response from the weather API containing weather and location data.
    /// </summary>
    public class WeatherApiResponse
    {
        /// <summary>
        /// Gets or sets the latitude of the location.
        /// </summary>
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the location.
        /// </summary>
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the time taken to generate the response in milliseconds.
        /// </summary>
        [JsonPropertyName("generationtime_ms")]
        public float GenerationTimeMs { get; set; }

        /// <summary>
        /// Gets or sets the UTC offset in seconds.
        /// </summary>
        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        /// <summary>
        /// Gets or sets the timezone of the location.
        /// </summary>
        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        /// <summary>
        /// Gets or sets the timezone abbreviation.
        /// </summary>
        [JsonPropertyName("timezone_abbreviation")]
        public string? TimezoneAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets the elevation of the location in meters.
        /// </summary>
        [JsonPropertyName("elevation")]
        public float Elevation { get; set; }

        /// <summary>
        /// Gets or sets the current weather conditions.
        /// </summary>
        [JsonPropertyName("current")]
        public CurrentWeather? CurrentWeather { get; set; }

        /// <summary>
        /// Gets or sets the hourly weather data.
        /// </summary>
        [JsonPropertyName("hourly")]
        public HourlyData? Hourly { get; set; }

        /// <summary>
        /// Gets or sets the daily weather data.
        /// </summary>
        [JsonPropertyName("daily")]
        public DailyData? Daily { get; set; }
    }

    /// <summary>
    /// Represents the current weather conditions.
    /// </summary>
    public class CurrentWeather
    {
        /// <summary>
        /// Gets or sets the temperature at 2 meters above ground level in degrees Celsius.
        /// </summary>
        [JsonPropertyName("temperature_2m")]
        public float Temperature { get; set; }

        /// <summary>
        /// Gets or sets the wind speed at 10 meters above ground level in meters per second.
        /// </summary>
        [JsonPropertyName("windspeed_10m")]
        public float WindSpeed { get; set; }

        /// <summary>
        /// Gets or sets the wind direction at 10 meters above ground level in degrees.
        /// </summary>
        [JsonPropertyName("winddirection_10m")]
        public int WindDirection { get; set; }

        /// <summary>
        /// Gets or sets the time of the current weather observation.
        /// </summary>
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// Represents the hourly weather data.
    /// </summary>
    public class HourlyData
    {
        /// <summary>
        /// Gets or sets the times of the hourly observations.
        /// </summary>
        [JsonPropertyName("time")]
        public List<DateTime>? Time { get; set; }

        /// <summary>
        /// Gets or sets the hourly temperatures at 2 meters above ground level in degrees Celsius.
        /// </summary>
        [JsonPropertyName("temperature_2m")]
        public List<float>? Temperature2M { get; set; }

        /// <summary>
        /// Gets or sets the hourly wind speeds at 10 meters above ground level in meters per second.
        /// </summary>
        [JsonPropertyName("windspeed_10m")]
        public List<float>? WindSpeed10M { get; set; }

        /// <summary>
        /// Gets or sets the hourly wind directions at 10 meters above ground level in degrees.
        /// </summary>
        [JsonPropertyName("winddirection_10m")]
        public List<int>? WindDirection10M { get; set; }
    }

    /// <summary>
    /// Represents the daily weather data.
    /// </summary>
    public class DailyData
    {
        /// <summary>
        /// Gets or sets the dates of the daily observations.
        /// </summary>
        [JsonPropertyName("time")]
        public List<string>? Time { get; set; }

        /// <summary>
        /// Gets or sets the sunrise times for each day.
        /// </summary>
        [JsonPropertyName("sunrise")]
        public List<string>? Sunrise { get; set; }

        /// <summary>
        /// Gets or sets the sunset times for each day.
        /// </summary>
        [JsonPropertyName("sunset")]
        public List<string>? Sunset { get; set; }
    }
}
