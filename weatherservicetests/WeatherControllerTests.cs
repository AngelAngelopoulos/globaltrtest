using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using weatherservice.Controllers;
using weatherservice.Models;
using weatherservice.Services;
using Xunit;

namespace weatherservice
{
    public class WeatherControllerTests
    {
        private readonly WeatherController _controller;
        private readonly WeatherByCityController _cityController;
        private readonly Mock<WeatherApiService> _mockWeatherService;

        public WeatherControllerTests()
        {
            _mockWeatherService = new Mock<WeatherApiService>();
            _controller = new WeatherController(_mockWeatherService.Object);
            _cityController = new WeatherByCityController(_mockWeatherService.Object);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns BadRequest when latitude is null.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsBadRequest_WhenLatitudeIsNull()
        {
            // Act
            ActionResult<WeatherResponse> result = await _controller.GetWeatherByLocation(null, 30);
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, badResult!.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns BadRequest when longitude is null.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsBadRequest_WhenLongitudeIsNull()
        {
            // Act
            var result = await _controller.GetWeatherByLocation(30, null);
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, badResult!.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns BadRequest when latitude is out of range.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsBadRequest_WhenLatitudeIsOutOfRange()
        {
            // Act
            var result = await _controller.GetWeatherByLocation(100, 30);
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, badResult!.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns BadRequest when longitude is out of range.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsBadRequest_WhenLongitudeIsOutOfRange()
        {
            // Act
            var result = await _controller.GetWeatherByLocation(30, 200);
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, badResult!.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns weather data when cached data exists.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsWeatherData_WhenCachedDataExists()
        {
            // Arrange
            var cachedWeather = new WeatherResponse { Latitude = 30, Longitude = 30, Temperature = 25 };
            _mockWeatherService.Setup(s => s.RetrieveDataFromDatabaseByLocation(30, 30)).ReturnsAsync(cachedWeather);

            // Act
            var result = await _controller.GetWeatherByLocation(30, 30);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedWeather = Assert.IsType<WeatherResponse>(okResult.Value);
            Assert.Equal(cachedWeather, returnedWeather);
        }

        /// <summary>
        /// Tests that GetWeatherByLocation returns a Problem result when an exception occurs.
        /// </summary>
        [Fact]
        public async Task GetWeatherByLocation_ReturnsProblem_WhenExceptionOccurs()
        {
            // Arrange
            _mockWeatherService.Setup(s => s.GetWeatherByLocationAsync(It.IsAny<float>(), It.IsAny<float>(), It.IsAny<string>())).ThrowsAsync(new Exception("API Error"));

            // Act
            var result = await _controller.GetWeatherByLocation(30, 30);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByCity returns BadRequest when city is null.
        /// </summary>
        [Fact]
        public async Task GetWeatherByCity_ReturnsBadRequest_WhenCityIsNull()
        {
            // Act
            var result = await _cityController.GetWeatherByCity(null);
            var badResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, badResult!.StatusCode);
        }

        /// <summary>
        /// Tests that GetWeatherByCity returns BadRequest when city is too short.
        /// </summary>
        [Fact]
        public async Task GetWeatherByCity_ReturnsBadRequest_WhenCityIsTooShort()
        {
            // Act
            var result = await _cityController.GetWeatherByCity("A");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("City must contain at least 2 letters", badRequestResult.Value!.GetType().GetProperties().First(o => o.Name == "message").GetValue(badRequestResult.Value, null));
        }

        /// <summary>
        /// Tests that GetWeatherByCity returns weather data when cached data exists.
        /// </summary>
        [Fact]
        public async Task GetWeatherByCity_ReturnsWeatherData_WhenCachedDataExists()
        {
            // Arrange
            var cachedWeather = new WeatherResponse { City = "New York", Temperature = 25 };
            _mockWeatherService.Setup(s => s.RetrieveDataFromDatabaseByCity("New York")).ReturnsAsync(cachedWeather);

            // Act
            var result = await _cityController.GetWeatherByCity("New York");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedWeather = Assert.IsType<WeatherResponse>(okResult.Value);
            Assert.Equal(cachedWeather, returnedWeather);
        }

        /// <summary>
        /// Tests that GetWeatherByCity returns NotFound when weather data for the city is not found.
        /// </summary>
        [Fact]
        public async Task GetWeatherByCity_ReturnsNotFound_WhenWeatherDataNotFound()
        {
            // Arrange
            _mockWeatherService.Setup(s => s.GetCityLocation(It.IsAny<string>())).ThrowsAsync(new Exception("City not found"));

            // Act
            var result = await _cityController.GetWeatherByCity("Unknown City");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("City not found", notFoundResult.Value!.GetType().GetProperties().First(o => o.Name == "detail").GetValue(notFoundResult.Value, null));
        }

        /// <summary>
        /// Tests that GetWeatherByCity returns a Problem result when an exception occurs.
        /// </summary>
        [Fact]
        public async Task GetWeatherByCity_ReturnsProblem_WhenExceptionOccurs()
        {
            // Arrange
            _mockWeatherService.Setup(s => s.GetWeatherByLocationAsync(It.IsAny<float>(), It.IsAny<float>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("API Error"));

            // Act
            var result = await _cityController.GetWeatherByCity("New York");

            // Assert
            var problemResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, problemResult.StatusCode);
            Assert.Equal("Weather or city data not found", problemResult.Value!.GetType().GetProperties().First(o => o.Name == "message").GetValue(problemResult.Value, null));
        }
    }
}
