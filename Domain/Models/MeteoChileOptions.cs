namespace WeatherMCP.Domain.Models
{
    public class MeteoChileOptions
    {
        public const string SectionName = "MeteoChile";
        
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }
}