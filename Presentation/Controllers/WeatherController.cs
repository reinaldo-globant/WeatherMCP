using Microsoft.AspNetCore.Mvc;
using WeatherMCP.Application.Interfaces;
using WeatherMCP.Domain.Models;

namespace WeatherMCP.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly IMeteoChileApiClient _apiClient;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IMeteoChileApiClient apiClient, ILogger<WeatherController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el catastro de estaciones meteorológicas disponibles
        /// </summary>
        /// <remarks>
        /// Retorna una lista completa de todas las estaciones meteorológicas disponibles en la red de MeteoChile,
        /// incluyendo información como código, nombre, ubicación geográfica y altitud.
        /// </remarks>
        /// <returns>Lista de estaciones meteorológicas</returns>
        /// <response code="200">Lista de estaciones obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("stations")]
        [ProducesResponseType(typeof(WeatherStationExample[]), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<IActionResult> GetWeatherStations()
        {
            try
            {
                var result = await _apiClient.GetWeatherStationsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather stations");
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los metadatos de una estación específica
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Metadatos de la estación</returns>
        [HttpGet("stations/{stationCode}/metadata")]
        public async Task<IActionResult> GetStationMetadata(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetStationMetadataAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station metadata for {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos minutarios de las 12 horas más recientes de todas las estaciones automáticas
        /// </summary>
        /// <returns>Datos recientes de todas las estaciones</returns>
        [HttpGet("stations/recent-data")]
        public async Task<IActionResult> GetRecentDataAllStations()
        {
            try
            {
                var result = await _apiClient.GetRecentDataAllStationsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent data for all stations");
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos minutarios de las 12 horas más recientes de una estación específica
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Datos recientes de la estación</returns>
        [HttpGet("stations/{stationCode}/recent-data")]
        public async Task<IActionResult> GetStationRecentData(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetStationRecentDataAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent data for station {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos cada 15 minutos de una estación para un mes específico
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <param name="year">Año (ej: 2024)</param>
        /// <param name="month">Mes (1-12)</param>
        /// <returns>Datos mensuales de la estación</returns>
        [HttpGet("stations/{stationCode}/monthly-data/{year}/{month}")]
        public async Task<IActionResult> GetStationMonthlyData(string stationCode, int year, int month)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { error = "El mes debe estar entre 1 y 12" });
            }

            try
            {
                var result = await _apiClient.GetStationMonthlyDataAsync(stationCode, year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly data for station {StationCode}, year {Year}, month {Month}", 
                    stationCode, year, month);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos del índice de radiación ultravioleta de la red nacional
        /// </summary>
        /// <returns>Datos del índice UV</returns>
        [HttpGet("uv-index")]
        public async Task<IActionResult> GetUvIndexData()
        {
            try
            {
                var result = await _apiClient.GetUvIndexDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UV index data");
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene resumen diario de todas las estaciones automáticas
        /// </summary>
        /// <returns>Resumen diario de todas las estaciones</returns>
        [HttpGet("stations/daily-summary")]
        public async Task<IActionResult> GetDailySummaryAllStations()
        {
            try
            {
                var result = await _apiClient.GetDailySummaryAllStationsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily summary for all stations");
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene resumen diario de una estación específica
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Resumen diario de la estación</returns>
        [HttpGet("stations/{stationCode}/daily-summary")]
        public async Task<IActionResult> GetStationDailySummary(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetStationDailySummaryAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily summary for station {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el boletín climatológico diario de las principales estaciones
        /// </summary>
        /// <returns>Boletín climatológico</returns>
        [HttpGet("climatological-bulletin")]
        public async Task<IActionResult> GetClimatologicalBulletin()
        {
            try
            {
                var result = await _apiClient.GetClimatologicalBulletinAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting climatological bulletin");
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }
    }
}