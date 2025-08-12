using System.Text.Json;
using Microsoft.Extensions.Options;
using WeatherMCP.Application.Interfaces;
using WeatherMCP.Domain.Models;

namespace WeatherMCP.Infrastructure.ExternalServices
{
    public class MeteoChileApiClient : IMeteoChileApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MeteoChileApiClient> _logger;
        private readonly MeteoChileOptions _options;

        public MeteoChileApiClient(HttpClient httpClient, ILogger<MeteoChileApiClient> logger, IOptions<MeteoChileOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
            
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        }

        public async Task<object> GetClimatologicalBulletinAsync()
        {
            return await GetAsync("getBoletinClimatologico");
        }

        public async Task<object> GetDailySummaryAllStationsAsync()
        {
            return await GetAsync("getResumenDiarioEstaciones");
        }

        public async Task<object> GetHistoricalPrecipitationDailyAsync(string stationCode, int year)
        {
            return await GetAsync($"getHistorialPrecipitacionDiaria/{stationCode}/{year}");
        }

        public async Task<object> GetHistoricalPrecipitationMonthlyAsync(string stationCode)
        {
            return await GetAsync($"getHistorialPrecipitacionMensual/{stationCode}");
        }

        public async Task<object> GetHistoricalPressureDailyAsync(string stationCode, int year)
        {
            return await GetAsync($"getHistorialPresionDiaria/{stationCode}/{year}");
        }

        public async Task<object> GetHistoricalPressureMonthlyAsync(string stationCode)
        {
            return await GetAsync($"getHistorialPresionMensual/{stationCode}");
        }

        public async Task<object> GetHistoricalTemperatureDailyAsync(string stationCode, int year)
        {
            return await GetAsync($"getHistorialTemperaturaDiaria/{stationCode}/{year}");
        }

        public async Task<object> GetHistoricalTemperatureMonthlyAsync(string stationCode)
        {
            return await GetAsync($"getHistorialTemperaturaMensual/{stationCode}");
        }

        public async Task<object> GetRecentDataAllStationsAsync()
        {
            return await GetAsync("getDatosRecientesEstaciones");
        }

        public async Task<object> GetStationDailySummaryAsync(string stationCode)
        {
            return await GetAsync($"getResumenDiarioEstacion/{stationCode}");
        }

        public async Task<object> GetStationMetadataAsync(string stationCode)
        {
            return await GetAsync($"getMetadatosEstacion/{stationCode}");
        }

        public async Task<object> GetStationMonthlyDataAsync(string stationCode, int year, int month)
        {
            return await GetAsync($"getDatosMensualesEstacion/{stationCode}/{year}/{month}");
        }

        public async Task<object> GetStationRecentDataAsync(string stationCode)
        {
            return await GetAsync($"getDatosRecientesEstacion/{stationCode}");
        }

        public async Task<object> GetUvIndexDataAsync()
        {
            return await GetAsync("getIndiceUV");
        }

        public async Task<object> GetWeatherStationsAsync()
        {
            return await GetAsync("getCatastroEstaciones");
        }

        private async Task<object> GetAsync(string endpoint)
        {
            try
            {
                var url = $"{_options.BaseUrl}{endpoint}";
                _logger.LogInformation("Calling MeteoChile API: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(content) ?? new { };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling MeteoChile API endpoint: {Endpoint}", endpoint);
                throw;
            }
        }
    }
}