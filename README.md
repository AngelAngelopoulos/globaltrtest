# Weather Service API

This project is a .NET Core Web API that provides weather information based on geographical location (latitude and longitude) or city name. The API fetches weather data from an external service and caches the data in a database for faster future retrieval.

## Features

- **Get Weather by Location**: Fetch weather data by specifying latitude and longitude.
- **Get Weather by City**: Fetch weather data by specifying a city name.
- **Error Handling**: Handles and returns meaningful error messages for invalid input and internal errors.
- **Caching**: Cached weather data for improved performance.

## Prerequisites

- **.NET 6 SDK** or later
- **Visual Studio 2022** or later
- **MongoDB** (for caching weather data)
- **OpenMeteo API Key** (or any weather API you are using)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/weatherservice.git
cd weatherservice
```

### 2. Open the Project in Visual Studio

1. Open Visual Studio.
2. Select **File > Open > Project/Solution**.
3. Navigate to the cloned repository and open the `weatherservice.sln` file.

### 3. Configure the Project

Before running the project, ensure you have the correct configuration settings.

1. Open the `appsettings.json` file.
2. Add your MongoDB connection string and database name under the `"ConnectionStrings"` section.
3. Add your weather API key under the `"WeatherApi"` section (if applicable).

Example `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDb": "your-mongodb-connection-string"
  },
  "WeatherApi": {
    "BaseUrl": "https://api.open-meteo.com/v1/forecast",
    "ApiKey": "your-api-key"
  }
}
```

### 4. Build the Project

- In Visual Studio, right-click the solution in the Solution Explorer and select **Build Solution**.
- Ensure there are no build errors before proceeding.

### 5. Run the Project

- Press **F5** or click on the **Start Debugging** button in Visual Studio to run the project.
- The API will start, and you should see the Swagger UI page at `https://localhost:<port>/swagger` for testing the endpoints.

## API Endpoints

### Get Weather by Location

- **Endpoint**: `GET /weather/location`
- **Parameters**:
  - `latitude` (float): The latitude of the location.
  - `longitude` (float): The longitude of the location.
- **Responses**:
  - `200 OK`: Returns the weather data for the specified location.
  - `400 Bad Request`: Returns an error if the latitude or longitude is invalid.
  - `500 Internal Server Error`: Returns an error if there is an issue with the external API or the service.

### Get Weather by City

- **Endpoint**: `GET /weather/city`
- **Parameters**:
  - `city` (string): The name of the city.
- **Responses**:
  - `200 OK`: Returns the weather data for the specified city.
  - `400 Bad Request`: Returns an error if the city name is invalid.
  - `404 Not Found`: Returns an error if the weather or city data is not found.
  - `500 Internal Server Error`: Returns an error if there is an issue with the external API or the service.

## Running Tests

The project includes unit tests for the API controllers. To run the tests:

1. Open the Test Explorer in Visual Studio by selecting **Test > Test Explorer**.
2. Click on **Run All** to execute all the tests.
3. Verify that all tests pass successfully.

## Deployment

To deploy the API:

1. Publish the project from Visual Studio by right-clicking the solution and selecting **Publish**.
2. Choose your target environment (e.g., Azure, IIS, Docker).
3. Follow the instructions to complete the deployment process.

## Contributing

If you wish to contribute to this project:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature-name`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/your-feature-name`).
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Acknowledgments

- Thanks to [OpenMeteo](https://open-meteo.com/) for providing the weather API used in this project.
- Special thanks to all contributors and users who provide feedback and suggestions.
