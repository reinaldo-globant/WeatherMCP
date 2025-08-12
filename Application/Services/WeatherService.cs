using WeatherMCP.Application.Interfaces;

namespace WeatherMCP.Application.Services
{
    public class WeatherService
    {
        private readonly IMeteoChileApiClient _apiClient;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IMeteoChileApiClient apiClient, ILogger<WeatherService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<object> GetWeatherStationsAsync()
        {
            _logger.LogInformation("Getting weather stations");
            return await _apiClient.GetWeatherStationsAsync();
        }

        public async Task<object> GetStationDataAsync(string stationCode)
        {
            _logger.LogInformation("Getting data for station: {StationCode}", stationCode);
            return await _apiClient.GetStationMetadataAsync(stationCode);
        }
    }
}