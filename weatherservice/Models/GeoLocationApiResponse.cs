using System.Text.Json.Serialization;

namespace weatherservice.Models
{
    /// <summary>
    /// Represents the response from the geolocation API containing latitude and longitude.
    /// </summary>
    public class GeoLocationApiResponse
    {
        /// <summary>
        /// Gets or sets the latitude of the location.
        /// </summary>
        /// <remarks>
        /// This property is mapped from the JSON field "latt".
        /// </remarks>
        [JsonPropertyName("latt")]
        public string Latt { get; set; } = "";

        /// <summary>
        /// Gets or sets the longitude of the location.
        /// </summary>
        /// <remarks>
        /// This property is mapped from the JSON field "longt".
        /// </remarks>
        [JsonPropertyName("longt")]
        public string Longt { get; set; } = "";
    }
}
