# WeatherMCP - MeteoChile API MCP Server

Un servidor híbrido que proporciona acceso a la API de MeteoChile para obtener datos meteorológicos de Chile. Funciona tanto como:

- **Servidor MCP (Model Context Protocol)** para integraciones con LLMs
- **API REST con OpenAPI/Swagger** para consumo directo por agentes y aplicaciones

## Configuración

### Variables de Entorno

El proyecto utiliza variables de entorno para la configuración. Puedes usar cualquiera de estos métodos:

1. **Archivo .env** (copia `.env.example` a `.env` y modifica los valores)
2. **Variables de entorno del sistema**
3. **Configuración en `appsettings.json`**

### Variables Disponibles

#### MeteoChile API Configuration
- `MeteoChile__BaseUrl`: URL base de la API de MeteoChile (default: `https://climatologia.meteochile.gob.cl/application/servicios/json/`)
- `MeteoChile__TimeoutSeconds`: Timeout en segundos para las peticiones HTTP (default: `30`)

#### MCP Server Configuration
- `McpServer__Name`: Nombre del servidor MCP (default: `meteochile-mcp-server`)
- `McpServer__Version`: Versión del servidor (default: `1.0.0`)
- `McpServer__ProtocolVersion`: Versión del protocolo MCP (default: `2024-11-05`)

#### ASP.NET Core Configuration
- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución (`Development`, `Production`, etc.)
- `ASPNETCORE_URLS`: URLs donde el servidor escuchará (default: `http://localhost:5175`)

#### Logging Configuration
- `Logging__LogLevel__Default`: Nivel de log por defecto
- `Logging__LogLevel__Microsoft.AspNetCore`: Nivel de log para ASP.NET Core

### Ejemplo de uso con variables de entorno

```bash
# Configurar variables de entorno
export MeteoChile__BaseUrl="https://api.custom.meteochile.cl/json/"
export MeteoChile__TimeoutSeconds=60
export McpServer__Name="my-weather-server"
export ASPNETCORE_ENVIRONMENT=Production

# Ejecutar la aplicación
dotnet run
```

### Docker

> 📋 **¿Necesitas ayuda configurando Docker?** Ver la [Guía de Configuración Docker](DOCKER_SETUP.md) para instrucciones detalladas de Colima y Docker Desktop.

#### Construcción y ejecución con Docker

```bash
# Desarrollo - Dockerfile estándar
docker build -t weathermcp .

# Producción - Dockerfile optimizado (Alpine Linux)
docker build -f Dockerfile.production -t weathermcp:production .

# Ejecutar con Docker
docker run -d \
  -e MeteoChile__BaseUrl="https://climatologia.meteochile.gob.cl/application/servicios/json/" \
  -e MeteoChile__TimeoutSeconds=45 \
  -e McpServer__Name="docker-weather-server" \
  -p 5175:8080 \
  weathermcp

# Acceder a la aplicación
# http://localhost:5175/swagger
```

#### Docker Compose (Recomendado)

```bash
# Desarrollo
docker-compose -f docker-compose.dev.yml up -d

# Producción
docker-compose up -d

# Ver logs
docker-compose logs -f weathermcp

# Verificar salud
docker-compose exec weathermcp ./healthcheck.sh

# Detener
docker-compose down
```

#### Características Docker

- **✅ .NET 9.0** - Usa las imágenes oficiales más recientes
- **✅ Multi-stage build** - Optimiza el tamaño de la imagen
- **✅ Non-root user** - Ejecuta con usuario no privilegiado
- **✅ Health checks** - Monitoreo automático del estado
- **✅ Alpine Linux** - Imagen de producción ultra liviana
- **✅ Variables de entorno** - Configuración flexible

## Estructura del Proyecto

```
WeatherMCP/
├── Application/
│   ├── Interfaces/          # Contratos de aplicación
│   └── Services/           # Servicios de dominio
├── Domain/
│   └── Models/             # Modelos de configuración
├── Infrastructure/
│   ├── ExternalServices/   # Clientes de APIs externas
│   └── MCP/               # Componentes del protocolo MCP
├── Presentation/
│   └── Controllers/       # Controladores HTTP (futuro)
└── Program.cs             # Punto de entrada
```

## Funcionalidades

### 🌐 API REST con OpenAPI/Swagger

Accede a la documentación interactiva en: **http://localhost:5175/swagger**

#### Endpoints HTTP Disponibles

**Endpoints del Sistema:**
- `GET /` - Redirige a la documentación Swagger
- `GET /health` - Endpoint de salud del servidor

**Endpoints de Datos Meteorológicos:**
- `GET /api/weather/stations` - Catastro de estaciones meteorológicas
- `GET /api/weather/stations/{code}/metadata` - Metadatos de una estación
- `GET /api/weather/stations/recent-data` - Datos recientes de todas las estaciones
- `GET /api/weather/stations/{code}/recent-data` - Datos recientes de una estación
- `GET /api/weather/stations/{code}/monthly-data/{year}/{month}` - Datos mensuales
- `GET /api/weather/uv-index` - Índice de radiación UV
- `GET /api/weather/stations/daily-summary` - Resumen diario de todas las estaciones
- `GET /api/weather/stations/{code}/daily-summary` - Resumen diario de una estación
- `GET /api/weather/climatological-bulletin` - Boletín climatológico

**Endpoints de Datos Históricos:**
- `GET /api/historical/temperature/monthly/{code}` - Temperatura histórica mensual
- `GET /api/historical/temperature/daily/{code}/{year}` - Temperatura histórica diaria
- `GET /api/historical/precipitation/monthly/{code}` - Precipitación histórica mensual
- `GET /api/historical/precipitation/daily/{code}/{year}` - Precipitación histórica diaria
- `GET /api/historical/pressure/monthly/{code}` - Presión histórica mensual
- `GET /api/historical/pressure/daily/{code}/{year}` - Presión histórica diaria

#### Ejemplo de uso con cURL

```bash
# Obtener todas las estaciones
curl -X GET "http://localhost:5175/api/weather/stations" -H "accept: application/json"

# Obtener datos recientes de una estación específica
curl -X GET "http://localhost:5175/api/weather/stations/330001/recent-data" -H "accept: application/json"

# Obtener datos históricos de temperatura diaria
curl -X GET "http://localhost:5175/api/historical/temperature/daily/330001/2024" -H "accept: application/json"
```

### 🔌 Servidor MCP (Model Context Protocol)

Compatible con integraciones directas con LLMs que soporten el protocolo MCP.

> 🤖 **¿Quieres usar WeatherMCP con Claude Desktop?** Ver la [Guía de Integración con Claude](MCP_CLAUDE_SETUP.md) para configuración paso a paso.

#### Herramientas MCP Disponibles

- `get_weather_stations` - Obtener catastro de estaciones
- `get_station_metadata` - Metadatos de una estación específica
- `get_recent_data_all_stations` - Datos recientes de todas las estaciones
- `get_station_recent_data` - Datos recientes de una estación
- `get_station_monthly_data` - Datos mensuales de una estación
- `get_uv_index_data` - Índice de radiación UV
- `get_daily_summary_all_stations` - Resumen diario de todas las estaciones
- `get_station_daily_summary` - Resumen diario de una estación
- `get_climatological_bulletin` - Boletín climatológico
- Funciones históricas de temperatura, precipitación y presión

#### Quick Start con Claude Desktop

```bash
# 1. Ejecutar WeatherMCP localmente
dotnet run

# 2. Configurar claude_desktop_config.json
# Ver MCP_CLAUDE_SETUP.md para detalles

# 3. Reiniciar Claude Desktop

# 4. ¡Preguntar sobre el clima en Chile!
```

## Desarrollo

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar en modo desarrollo
dotnet run --environment Development

# Ejecutar con configuración personalizada
dotnet run --configuration Release
```