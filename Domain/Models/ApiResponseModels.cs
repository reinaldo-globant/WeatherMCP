namespace WeatherMCP.Domain.Models
{
    /// <summary>
    /// Respuesta de error estándar de la API
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Mensaje de error
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Ejemplo de estación meteorológica
    /// </summary>
    public class WeatherStationExample
    {
        /// <summary>
        /// Código de la estación
        /// </summary>
        public string Code { get; set; } = "330001";

        /// <summary>
        /// Nombre de la estación
        /// </summary>
        public string Name { get; set; } = "ARICA";

        /// <summary>
        /// Latitud
        /// </summary>
        public double Latitude { get; set; } = -18.4746;

        /// <summary>
        /// Longitud
        /// </summary>
        public double Longitude { get; set; } = -70.3127;

        /// <summary>
        /// Altitud en metros
        /// </summary>
        public int Altitude { get; set; } = 58;
    }

    /// <summary>
    /// Ejemplo de datos meteorológicos recientes
    /// </summary>
    public class RecentWeatherDataExample
    {
        /// <summary>
        /// Código de la estación
        /// </summary>
        public string StationCode { get; set; } = "330001";

        /// <summary>
        /// Fecha y hora de la medición
        /// </summary>
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Temperatura en grados Celsius
        /// </summary>
        public double? Temperature { get; set; } = 22.5;

        /// <summary>
        /// Humedad relativa en porcentaje
        /// </summary>
        public double? Humidity { get; set; } = 65.3;

        /// <summary>
        /// Presión atmosférica en hPa
        /// </summary>
        public double? Pressure { get; set; } = 1013.2;

        /// <summary>
        /// Velocidad del viento en km/h
        /// </summary>
        public double? WindSpeed { get; set; } = 15.4;

        /// <summary>
        /// Dirección del viento en grados
        /// </summary>
        public double? WindDirection { get; set; } = 180.0;
    }

    /// <summary>
    /// Respuesta de verificación de salud
    /// </summary>
    public class HealthCheckResponse
    {
        /// <summary>
        /// Estado del servidor
        /// </summary>
        public string Status { get; set; } = "healthy";

        /// <summary>
        /// Timestamp de la verificación
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}