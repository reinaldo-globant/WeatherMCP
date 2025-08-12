namespace WeatherMCP.Application.Interfaces
{
    public interface IMeteoChileApiClient
    {
        Task<object> GetClimatologicalBulletinAsync();
        Task<object> GetDailySummaryAllStationsAsync();
        Task<object> GetHistoricalPrecipitationDailyAsync(string stationCode, int year);
        Task<object> GetHistoricalPrecipitationMonthlyAsync(string stationCode);
        Task<object> GetHistoricalPressureDailyAsync(string stationCode, int year);
        Task<object> GetHistoricalPressureMonthlyAsync(string stationCode);
        Task<object> GetHistoricalTemperatureDailyAsync(string stationCode, int year);
        Task<object> GetHistoricalTemperatureMonthlyAsync(string stationCode);
        Task<object> GetRecentDataAllStationsAsync();
        Task<object> GetStationDailySummaryAsync(string stationCode);
        Task<object> GetStationMetadataAsync(string stationCode);
        Task<object> GetStationMonthlyDataAsync(string stationCode, int year, int month);
        Task<object> GetStationRecentDataAsync(string stationCode);
        Task<object> GetUvIndexDataAsync();
        Task<object> GetWeatherStationsAsync();
    }
}