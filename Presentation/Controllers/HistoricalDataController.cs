using Microsoft.AspNetCore.Mvc;
using WeatherMCP.Application.Interfaces;
using WeatherMCP.Domain.Models;

namespace WeatherMCP.Presentation.Controllers
{
    [ApiController]
    [Route("api/historical")]
    [Produces("application/json")]
    public class HistoricalDataController : ControllerBase
    {
        private readonly IMeteoChileApiClient _apiClient;
        private readonly ILogger<HistoricalDataController> _logger;

        public HistoricalDataController(IMeteoChileApiClient apiClient, ILogger<HistoricalDataController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene datos históricos de temperatura mensual y anual
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Datos históricos de temperatura mensual</returns>
        [HttpGet("temperature/monthly/{stationCode}")]
        public async Task<IActionResult> GetHistoricalTemperatureMonthly(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetHistoricalTemperatureMonthlyAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical monthly temperature for station {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos históricos de temperatura diaria para un año específico
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <param name="year">Año (ej: 2024)</param>
        /// <returns>Datos históricos de temperatura diaria</returns>
        [HttpGet("temperature/daily/{stationCode}/{year}")]
        public async Task<IActionResult> GetHistoricalTemperatureDaily(string stationCode, int year)
        {
            if (year < 1900 || year > DateTime.Now.Year + 1)
            {
                return BadRequest(new ApiErrorResponse { Error = "Año no válido" });
            }

            try
            {
                var result = await _apiClient.GetHistoricalTemperatureDailyAsync(stationCode, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical daily temperature for station {StationCode}, year {Year}", 
                    stationCode, year);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos históricos de precipitación mensual y anual
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Datos históricos de precipitación mensual</returns>
        [HttpGet("precipitation/monthly/{stationCode}")]
        public async Task<IActionResult> GetHistoricalPrecipitationMonthly(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetHistoricalPrecipitationMonthlyAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical monthly precipitation for station {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos históricos de precipitación diaria para un año específico
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <param name="year">Año (ej: 2024)</param>
        /// <returns>Datos históricos de precipitación diaria</returns>
        [HttpGet("precipitation/daily/{stationCode}/{year}")]
        public async Task<IActionResult> GetHistoricalPrecipitationDaily(string stationCode, int year)
        {
            if (year < 1900 || year > DateTime.Now.Year + 1)
            {
                return BadRequest(new ApiErrorResponse { Error = "Año no válido" });
            }

            try
            {
                var result = await _apiClient.GetHistoricalPrecipitationDailyAsync(stationCode, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical daily precipitation for station {StationCode}, year {Year}", 
                    stationCode, year);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos históricos de presión a nivel del mar mensual y anual
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <returns>Datos históricos de presión mensual</returns>
        [HttpGet("pressure/monthly/{stationCode}")]
        public async Task<IActionResult> GetHistoricalPressureMonthly(string stationCode)
        {
            try
            {
                var result = await _apiClient.GetHistoricalPressureMonthlyAsync(stationCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical monthly pressure for station {StationCode}", stationCode);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene datos históricos de presión a nivel del mar diaria para un año específico
        /// </summary>
        /// <param name="stationCode">Código de la estación meteorológica</param>
        /// <param name="year">Año (ej: 2024)</param>
        /// <returns>Datos históricos de presión diaria</returns>
        [HttpGet("pressure/daily/{stationCode}/{year}")]
        public async Task<IActionResult> GetHistoricalPressureDaily(string stationCode, int year)
        {
            if (year < 1900 || year > DateTime.Now.Year + 1)
            {
                return BadRequest(new ApiErrorResponse { Error = "Año no válido" });
            }

            try
            {
                var result = await _apiClient.GetHistoricalPressureDailyAsync(stationCode, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting historical daily pressure for station {StationCode}, year {Year}", 
                    stationCode, year);
                return StatusCode(500, new ApiErrorResponse { Error = "Error interno del servidor" });
            }
        }
    }
}