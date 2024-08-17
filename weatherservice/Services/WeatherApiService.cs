using System;
using System.Text.Json;
using MongoDB.Driver;
using weatherservice.Models;
using System.Globalization;

namespace weatherservice.Services
{
    /// <summary>
    /// Provides weather-related services, including fetching weather data from external APIs and interacting with MongoDB.
    /// </summary>
    public class WeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMongoCollection<WeatherResponse> _weatherCollection;

        private static WeatherApiService? _instance;
        private static readonly object _lock = new();

        private readonly string format = "yyyy-MM-ddTHH:mm";

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApiService"/> class with a specified MongoDB client.
        /// </summary>
        /// <param name="mongoClient">The MongoDB client to use for database operations.</param>
        private WeatherApiService(IMongoClient mongoClient)
        {
            _httpClient = new HttpClient();
            var database = mongoClient.GetDatabase("WeatherService");
            _weatherCollection = database.GetCollection<WeatherResponse>("WeatherData");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApiService"/> class with a default MongoDB connection.
        /// </summary>
        public WeatherApiService()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            _httpClient = new HttpClient();
            var database = mongoClient.GetDatabase("WeatherService");
            _weatherCollection = database.GetCollection<WeatherResponse>("WeatherData");
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="WeatherApiService"/> class.
        /// </summary>
        /// <param name="mongoClient">The MongoDB client to use for database operations.</param>
        /// <returns>The singleton instance of the <see cref="WeatherApiService"/> class.</returns>
        public static WeatherApiService GetInstance(IMongoClient mongoClient)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new WeatherApiService(mongoClient);
                }
            }
            return _instance;
        }

        /// <summary>
        /// Retrieves weather data by geographical location (latitude and longitude).
        /// </summary>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <param name="city">The optional name of the city associated with the weather data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="WeatherResponse"/> as its result.</returns>
        /// <exception cref="Exception">Thrown when there is an error in deserializing the API response or parsing the date/time.</exception>
        public virtual async Task<WeatherResponse> GetWeatherByLocationAsync(float latitude, float longitude, string? city = null)
        {
            var response = await _httpClient.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,winddirection_10m,windspeed_10m&daily=sunrise");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content) ?? throw new Exception("Error trying to deserializing Api response");

                if (DateTime.TryParseExact(s: weatherApiResponse.Daily?.Sunrise?.FirstOrDefault(DateTime.Now.ToString()) ?? DateTime.Now.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sunriseDateTime))
                {
                    var weatherData = new WeatherResponse
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Temperature = weatherApiResponse.CurrentWeather?.Temperature,
                        WindDirection = weatherApiResponse.CurrentWeather?.WindDirection,
                        WindSpeed = weatherApiResponse.CurrentWeather?.WindSpeed,
                        SunriseDateTime = sunriseDateTime,
                        Timestamp = DateTime.Now,
                        City = city
                    };
                    await _weatherCollection.InsertOneAsync(weatherData);
                    return weatherData;
                }
                else
                {
                    throw new Exception("Error parsing date/time, bad format");
                }
            }

            throw new Exception("GetWeatherByLocationAsync Request error");
        }

        /// <summary>
        /// Retrieves the geographical coordinates (latitude and longitude) of a city.
        /// </summary>
        /// <param name="city">The name of the city to get the location for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="Tuple{float, float}"/> as its result containing the latitude and longitude.</returns>
        /// <exception cref="Exception">Thrown when the geolocation request fails or the data cannot be retrieved.</exception>
        public virtual async Task<Tuple<float, float>> GetCityLocation(string city)
        {
            var geoResponse = await _httpClient.GetAsync($"https://geocode.xyz/{city}?json=1");
            if (!geoResponse.IsSuccessStatusCode)
            {
                throw new Exception("Geolocation request error");
            }

            var geoContent = await geoResponse.Content.ReadAsStringAsync();
            var geoData = JsonSerializer.Deserialize<GeoLocationApiResponse>(geoContent);
            if (geoData == null || string.IsNullOrEmpty(geoData.Latt) || string.IsNullOrEmpty(geoData.Longt))
            {
                throw new Exception("Unable to retrieve geolocation data");
            }

            float latitude = float.Parse(geoData.Latt, CultureInfo.InvariantCulture);
            float longitude = float.Parse(geoData.Longt, CultureInfo.InvariantCulture);

            return new Tuple<float, float>(latitude, longitude);
        }

        /// <summary>
        /// Retrieves cached weather data from the database by city name.
        /// </summary>
        /// <param name="city">The name of the city to retrieve weather data for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="WeatherResponse"/> or null if not found.</returns>
        public virtual async Task<WeatherResponse?> RetrieveDataFromDatabaseByCity(string city)
        {
            var cachedData = await _weatherCollection
                .Find(w => w.City == city)
                .FirstOrDefaultAsync();

            return cachedData;
        }

        /// <summary>
        /// Retrieves cached weather data from the database by geographical location (latitude and longitude).
        /// </summary>
        /// <param name="latitude">The latitude of the location to retrieve weather data for.</param>
        /// <param name="longitude">The longitude of the location to retrieve weather data for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="WeatherResponse"/> or null if not found.</returns>
        public virtual async Task<WeatherResponse?> RetrieveDataFromDatabaseByLocation(float latitude, float longitude)
        {
            var cachedData = await _weatherCollection
                .Find(w => w.Latitude == latitude && w.Longitude == longitude)
                .FirstOrDefaultAsync();

            return cachedData;
        }
    }
}
