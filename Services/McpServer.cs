using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MeteoChileMcpServer
{
    public class McpServer
    {
        private readonly ILogger<McpServer> _logger;
        private readonly IMeteoChileApiClient _apiClient;
        private readonly Dictionary<string, Func<JsonElement, Task<object>>> _tools;

        public McpServer(ILogger<McpServer> logger, IMeteoChileApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
            _tools = InitializeTools();
        }

        private Dictionary<string, Func<JsonElement, Task<object>>> InitializeTools()
        {
            return new Dictionary<string, Func<JsonElement, Task<object>>>
            {
                ["get_weather_stations"] = async (args) => await _apiClient.GetWeatherStationsAsync(),
                ["get_station_metadata"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetStationMetadataAsync(stationCode);
                },
                ["get_recent_data_all_stations"] = async (args) => await _apiClient.GetRecentDataAllStationsAsync(),
                ["get_station_recent_data"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetStationRecentDataAsync(stationCode);
                },
                ["get_station_monthly_data"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    var year = args.GetProperty("year").GetInt32();
                    var month = args.GetProperty("month").GetInt32();
                    return await _apiClient.GetStationMonthlyDataAsync(stationCode, year, month);
                },
                ["get_uv_index_data"] = async (args) => await _apiClient.GetUvIndexDataAsync(),
                ["get_daily_summary_all_stations"] = async (args) => await _apiClient.GetDailySummaryAllStationsAsync(),
                ["get_station_daily_summary"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetStationDailySummaryAsync(stationCode);
                },
                ["get_climatological_bulletin"] = async (args) => await _apiClient.GetClimatologicalBulletinAsync(),
                ["get_historical_temperature_monthly"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetHistoricalTemperatureMonthlyAsync(stationCode);
                },
                ["get_historical_temperature_daily"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    var year = args.GetProperty("year").GetInt32();
                    return await _apiClient.GetHistoricalTemperatureDailyAsync(stationCode, year);
                },
                ["get_historical_precipitation_monthly"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetHistoricalPrecipitationMonthlyAsync(stationCode);
                },
                ["get_historical_precipitation_daily"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    var year = args.GetProperty("year").GetInt32();
                    return await _apiClient.GetHistoricalPrecipitationDailyAsync(stationCode, year);
                },
                ["get_historical_pressure_monthly"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    return await _apiClient.GetHistoricalPressureMonthlyAsync(stationCode);
                },
                ["get_historical_pressure_daily"] = async (args) =>
                {
                    var stationCode = args.GetProperty("station_code").GetString();
                    var year = args.GetProperty("year").GetInt32();
                    return await _apiClient.GetHistoricalPressureDailyAsync(stationCode, year);
                }
            };
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("MeteoChile MCP Server iniciado y esperando conexiones...");

            using var stdin = Console.OpenStandardInput();
            using var stdout = Console.OpenStandardOutput();
            using var reader = new StreamReader(stdin);
            using var writer = new StreamWriter(stdout) { AutoFlush = true };

            while (true)
            {
                try
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;

                    var request = JsonSerializer.Deserialize<McpRequest>(line);
                    var response = await HandleRequestAsync(request);
                    
                    var responseJson = JsonSerializer.Serialize(response);
                    await writer.WriteLineAsync(responseJson);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando request MCP");
                    var errorResponse = new McpResponse
                    {
                        Id = "error",
                        Error = new McpError
                        {
                            Code = -1,
                            Message = ex.Message
                        }
                    };
                    var errorJson = JsonSerializer.Serialize(errorResponse);
                    await writer.WriteLineAsync(errorJson);
                }
            }
        }

        private async Task<McpResponse> HandleRequestAsync(McpRequest request)
        {
            switch (request.Method)
            {
                case "initialize":
                    return new McpResponse
                    {
                        Id = request.Id,
                        Result = new
                        {
                            protocolVersion = "2024-11-05",
                            capabilities = new
                            {
                                tools = new { }
                            },
                            serverInfo = new
                            {
                                name = "meteochile-mcp-server",
                                version = "1.0.0"
                            }
                        }
                    };

                case "tools/list":
                    return new McpResponse
                    {
                        Id = request.Id,
                        Result = new
                        {
                            tools = GetToolDefinitions()
                        }
                    };

                case "tools/call":
                    var toolName = request.Params?.GetProperty("name").GetString();
                    var arguments = request.Params?.GetProperty("arguments");

                    if (_tools.TryGetValue(toolName, out var toolFunc))
                    {
                        try
                        {
                            var result = await toolFunc(arguments ?? new JsonElement());
                            return new McpResponse
                            {
                                Id = request.Id,
                                Result = new
                                {
                                    content = new[]
                                    {
                                        new
                                        {
                                            type = "text",
                                            text = JsonSerializer.Serialize(result, new JsonSerializerOptions 
                                            { 
                                                WriteIndented = true,
                                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                            })
                                        }
                                    }
                                }
                            };
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error ejecutando tool {ToolName}", toolName);
                            return new McpResponse
                            {
                                Id = request.Id,
                                Error = new McpError
                                {
                                    Code = -1,
                                    Message = $"Error ejecutando herramienta {toolName}: {ex.Message}"
                                }
                            };
                        }
                    }

                    return new McpResponse
                    {
                        Id = request.Id,
                        Error = new McpError
                        {
                            Code = -32601,
                            Message = $"Herramienta no encontrada: {toolName}"
                        }
                    };

                default:
                    return new McpResponse
                    {
                        Id = request.Id,
                        Error = new McpError
                        {
                            Code = -32601,
                            Message = $"Método no soportado: {request.Method}"
                        }
                    };
            }
        }

        private object[] GetToolDefinitions()
        {
            return new object[]
            {
                new
                {
                    name = "get_weather_stations",
                    description = "Obtiene el catastro de estaciones meteorológicas disponibles",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = new string[0]
                    }
                },
                new
                {
                    name = "get_station_metadata",
                    description = "Obtiene los metadatos de una estación específica",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_recent_data_all_stations",
                    description = "Obtiene datos minutarios de las 12 horas más recientes de todas las estaciones automáticas",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = new string[0]
                    }
                },
                new
                {
                    name = "get_station_recent_data",
                    description = "Obtiene datos minutarios de las 12 horas más recientes de una estación específica",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_station_monthly_data",
                    description = "Obtiene datos cada 15 minutos de una estación para un mes específico",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" },
                            year = new { type = "integer", description = "Año (ej: 2024)" },
                            month = new { type = "integer", description = "Mes (1-12)" }
                        },
                        required = new[] { "station_code", "year", "month" }
                    }
                },
                new
                {
                    name = "get_uv_index_data",
                    description = "Obtiene datos del índice de radiación ultravioleta de la red nacional",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = new string[0]
                    }
                },
                new
                {
                    name = "get_daily_summary_all_stations",
                    description = "Obtiene resumen diario de todas las estaciones automáticas",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = new string[0]
                    }
                },
                new
                {
                    name = "get_station_daily_summary",
                    description = "Obtiene resumen diario de una estación específica",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_climatological_bulletin",
                    description = "Obtiene el boletín climatológico diario de las principales estaciones",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = new string[0]
                    }
                },
                new
                {
                    name = "get_historical_temperature_monthly",
                    description = "Obtiene datos históricos de temperatura mensual y anual",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_historical_temperature_daily",
                    description = "Obtiene datos históricos de temperatura diaria para un año específico",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" },
                            year = new { type = "integer", description = "Año (ej: 2024)" }
                        },
                        required = new[] { "station_code", "year" }
                    }
                },
                new
                {
                    name = "get_historical_precipitation_monthly",
                    description = "Obtiene datos históricos de precipitación mensual y anual",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_historical_precipitation_daily",
                    description = "Obtiene datos históricos de precipitación diaria para un año específico",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" },
                            year = new { type = "integer", description = "Año (ej: 2024)" }
                        },
                        required = new[] { "station_code", "year" }
                    }
                },
                new
                {
                    name = "get_historical_pressure_monthly",
                    description = "Obtiene datos históricos de presión a nivel del mar mensual y anual",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" }
                        },
                        required = new[] { "station_code" }
                    }
                },
                new
                {
                    name = "get_historical_pressure_daily",
                    description = "Obtiene datos históricos de presión a nivel del mar diaria para un año específico",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            station_code = new { type = "string", description = "Código de la estación meteorológica" },
                            year = new { type = "integer", description = "Año (ej: 2024)" }
                        },
                        required = new[] { "station_code", "year" }
                    }
                }
            };
        }
    }
}