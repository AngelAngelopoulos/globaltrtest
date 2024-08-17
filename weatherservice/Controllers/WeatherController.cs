using Microsoft.AspNetCore.Mvc;
using weatherservice.Models;
using weatherservice.Services;

namespace weatherservice.Controllers;

[ApiController]
[Route("weather/location")]
public class WeatherController : ControllerBase
{
    private readonly WeatherApiService _weatherService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherController"/> class.
    /// </summary>
    /// <param name="weatherApiService">The weather API service.</param>
    public WeatherController(WeatherApiService weatherApiService)
    {
        _weatherService = weatherApiService;
    }

    /// <summary>
    /// Gets the weather by geographical location (latitude and longitude).
    /// </summary>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <returns>
    /// Returns the weather information for the specified location.
    /// Returns <see cref="BadRequest"/> if the latitude or longitude is invalid.
    /// Returns <see cref="Problem"/> if there is an internal error.
    /// </returns>
    [HttpGet(Name = "GetWeatherByLocation")]
    public async Task<ActionResult<WeatherResponse>> GetWeatherByLocation(float? latitude, float? longitude)
    {
        // Validate latitude and longitude
        if (latitude == null || longitude == null)
        {
            return BadRequest(new
            {
                message = "Latitude and Longitude are required",
                timestamp = DateTime.UtcNow,
            });
        }

        if (latitude < -90 || latitude > 90)
        {
            return BadRequest(new
            {
                message = "Latitude must be between -90 and 90.",
                latitude,
                timestamp = DateTime.UtcNow,
            });
        }

        if (longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                message = "Longitude must be between -180 and 180.",
                longitude,
                timestamp = DateTime.UtcNow,
            });
        }

        try
        {
            // Try to retrieve cached data
            WeatherResponse? cached = await _weatherService.RetrieveDataFromDatabaseByLocation((float)latitude!, (float)longitude!);
            if (cached != null) return Ok(cached);

            // If not cached, call the external API
            WeatherResponse weatherResponse = await _weatherService.GetWeatherByLocationAsync((float)latitude, (float)longitude);
            return Ok(weatherResponse);
        }
        catch (Exception e)
        {
            // Return a problem response if an exception occurs
            return Problem(detail: e.Message);
        }
    }
}

[ApiController]
[Route("weather/city")]
public class WeatherByCityController : ControllerBase
{
    private readonly WeatherApiService _weatherService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherByCityController"/> class.
    /// </summary>
    /// <param name="weatherApiService">The weather API service.</param>
    public WeatherByCityController(WeatherApiService weatherApiService)
    {
        _weatherService = weatherApiService;
    }

    /// <summary>
    /// Gets the weather by city name.
    /// </summary>
    /// <param name="city">The name of the city.</param>
    /// <returns>
    /// Returns the weather information for the specified city.
    /// Returns <see cref="BadRequest"/> if the city parameter is invalid.
    /// Returns <see cref="NotFound"/> if weather or city data is not found.
    /// Returns <see cref="Problem"/> if there is an internal error.
    /// </returns>
    [HttpGet(Name = "GetWeatherByCity")]
    public async Task<ActionResult<WeatherResponse>> GetWeatherByCity(string? city)
    {
        // Validate city parameter
        if (city == null)
        {
            return BadRequest(new
            {
                message = "City is required",
                timestamp = DateTime.UtcNow,
            });
        }

        if (city.Length < 2)
        {
            return BadRequest(new
            {
                message = "City must contain at least 2 letters",
                city,
                timestamp = DateTime.UtcNow,
            });
        }

        try
        {
            // Try to retrieve cached data
            WeatherResponse? cached = await _weatherService.RetrieveDataFromDatabaseByCity(city);
            if (cached != null) return Ok(cached);
        }
        catch (Exception e)
        {
            // Return a problem response if an exception occurs
            return Problem(detail: e.Message);
        }

        try
        {
            // Get city location (latitude and longitude)
            Tuple<float, float> location = await _weatherService.GetCityLocation(city);

            // Get weather data by location
            WeatherResponse weatherResponse = await _weatherService.GetWeatherByLocationAsync(location.Item1, location.Item2, city: city);
            return Ok(weatherResponse);
        }
        catch (Exception e)
        {
            // Return a NotFound response if weather or city data is not found
            return NotFound(new
            {
                message = "Weather or city data not found",
                city,
                detail = e.Message,
                timestamp = DateTime.UtcNow,
            });
        }
    }
}
