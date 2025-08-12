# Integración con Claude Desktop - WeatherMCP

Esta guía te ayudará a configurar WeatherMCP como servidor MCP en Claude Desktop para acceder a datos meteorológicos directamente desde Claude.

## 🚀 Despliegue Local

### Paso 1: Ejecutar WeatherMCP Localmente

#### Opción A: Con .NET (Desarrollo)
```bash
# Clonar el repositorio
git clone https://github.com/tu-usuario/WeatherMCP.git
cd WeatherMCP

# Restaurar dependencias
dotnet restore

# Ejecutar en modo desarrollo
dotnet run --environment Development

# El servidor estará disponible en:
# - HTTP API: http://localhost:5175/swagger
# - MCP Server: stdin/stdout del proceso
```

#### Opción B: Con Docker (Recomendado)
```bash
# Ejecutar con Docker Compose
docker-compose -f docker-compose.dev.yml up -d

# Ver logs
docker-compose logs -f

# El servidor MCP estará corriendo dentro del contenedor
```

### Paso 2: Verificar que el Servidor Funciona

```bash
# Verificar API REST
curl http://localhost:5175/health

# Debería devolver:
# {"status":"healthy","timestamp":"2024-01-15T10:30:00.000Z"}
```

## 🔌 Configuración de Claude Desktop

### Paso 1: Ubicar el Archivo de Configuración

El archivo de configuración de Claude Desktop se encuentra en:

**macOS:**
```bash
~/Library/Application\ Support/Claude/claude_desktop_config.json
```

**Windows:**
```bash
%APPDATA%/Claude/claude_desktop_config.json
```

**Linux:**
```bash
~/.config/Claude/claude_desktop_config.json
```

### Paso 2: Configurar el Servidor MCP

#### Para ejecución con .NET (desarrollo)

Edita `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "weathermcp": {
      "command": "dotnet",
      "args": ["run", "--project", "/ruta/completa/a/WeatherMCP"],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "MeteoChile__BaseUrl": "https://climatologia.meteochile.gob.cl/application/servicios/json/",
        "MeteoChile__TimeoutSeconds": "30",
        "McpServer__Name": "meteochile-mcp-server",
        "McpServer__Version": "1.0.0",
        "McpServer__ProtocolVersion": "2024-11-05"
      }
    }
  }
}
```

#### Para ejecución con Docker

```json
{
  "mcpServers": {
    "weathermcp": {
      "command": "docker",
      "args": [
        "exec", 
        "weathermcp-dev", 
        "dotnet", 
        "WeatherMCP.dll"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    }
  }
}
```

#### Configuración Simplificada (Binario compilado)

Si prefieres compilar un binario:

```bash
# Compilar binario auto-contenido
dotnet publish -c Release -r osx-arm64 --self-contained -o ./publish

# O para tu plataforma específica
dotnet publish -c Release -r win-x64 --self-contained -o ./publish    # Windows
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish  # Linux
```

Luego configura Claude:

```json
{
  "mcpServers": {
    "weathermcp": {
      "command": "/ruta/completa/a/publish/WeatherMCP",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Production",
        "MeteoChile__BaseUrl": "https://climatologia.meteochile.gob.cl/application/servicios/json/"
      }
    }
  }
}
```

### Paso 3: Reiniciar Claude Desktop

1. **Cierra** completamente Claude Desktop
2. **Abre** Claude Desktop nuevamente
3. **Verifica** que el servidor MCP se haya cargado correctamente

## 🧪 Pruebas de Funcionalidad

### Verificar Conexión MCP

En una conversación con Claude, deberías poder hacer preguntas como:

```
¿Puedes mostrarme las estaciones meteorológicas disponibles en Chile?
```

```
¿Cuál es el clima actual en la estación de Arica?
```

```
Necesito datos históricos de temperatura de Santiago para el año 2023
```

### Herramientas MCP Disponibles

Una vez configurado, Claude tendrá acceso a estas herramientas:

| Herramienta | Descripción | Parámetros |
|-------------|-------------|------------|
| `get_weather_stations` | Catastro de estaciones | Ninguno |
| `get_station_metadata` | Metadatos de estación | `station_code` |
| `get_recent_data_all_stations` | Datos recientes globales | Ninguno |
| `get_station_recent_data` | Datos recientes específicos | `station_code` |
| `get_station_monthly_data` | Datos mensuales | `station_code`, `year`, `month` |
| `get_uv_index_data` | Índice UV nacional | Ninguno |
| `get_climatological_bulletin` | Boletín climatológico | Ninguno |
| `get_historical_temperature_*` | Datos históricos temperatura | `station_code`, `year` |
| `get_historical_precipitation_*` | Datos históricos precipitación | `station_code`, `year` |
| `get_historical_pressure_*` | Datos históricos presión | `station_code`, `year` |

## 🔧 Troubleshooting

### Problema: Claude no reconoce el servidor MCP

**Posibles causas:**
1. Ruta incorrecta en la configuración
2. Permisos de archivo
3. Servidor MCP no está ejecutándose

**Soluciones:**
```bash
# Verificar que el archivo de config existe
ls -la ~/Library/Application\ Support/Claude/claude_desktop_config.json

# Verificar sintaxis JSON
cat ~/Library/Application\ Support/Claude/claude_desktop_config.json | python -m json.tool

# Probar ejecutar el comando manualmente
dotnet run --project /ruta/completa/a/WeatherMCP
```

### Problema: Servidor MCP se inicia pero no responde

**Verificar logs:**

Para .NET:
```bash
# Ver logs en la consola donde ejecutaste dotnet run
dotnet run --environment Development
```

Para Docker:
```bash
# Ver logs del contenedor
docker logs weathermcp-dev -f
```

### Problema: Error de conectividad con MeteoChile API

**Verificar conexión:**
```bash
# Probar API de MeteoChile directamente
curl "https://climatologia.meteochile.gob.cl/application/servicios/json/getCatastroEstaciones"
```

### Problema: Claude Desktop no inicia después de la configuración

**Verificar configuración JSON:**
```bash
# Validar sintaxis JSON
python -c "import json; print(json.load(open('claude_desktop_config.json')))"
```

**Backup y reset:**
```bash
# Hacer backup de configuración
cp ~/Library/Application\ Support/Claude/claude_desktop_config.json ~/Desktop/claude_config_backup.json

# Reset a configuración vacía
echo '{"mcpServers": {}}' > ~/Library/Application\ Support/Claude/claude_desktop_config.json
```

## 🎯 Ejemplos de Uso con Claude

### Consulta Básica
```
Usuario: ¿Qué estaciones meteorológicas hay disponibles?

Claude: Te ayudo a obtener el catastro de estaciones meteorológicas. Déjame consultar la información disponible.

[Claude usa get_weather_stations]

Aquí tienes las estaciones meteorológicas disponibles en Chile:

1. **ARICA** (330001)
   - Latitud: -18.4746°, Longitud: -70.3127°
   - Altitud: 58m

2. **IQUIQUE** (330002)
   - Latitud: -20.5354°, Longitud: -70.1626°
   - Altitud: 51m

[... más estaciones ...]
```

### Consulta de Datos Específicos
```
Usuario: Necesito los datos meteorológicos actuales de la estación de Arica

Claude: Te voy a obtener los datos meteorológicos más recientes de la estación de Arica.

[Claude usa get_station_recent_data con station_code="330001"]

Datos meteorológicos recientes de ARICA:

**Última medición:** 15/01/2024 - 14:30 hrs
- **Temperatura:** 24.5°C
- **Humedad:** 65%
- **Presión:** 1013.2 hPa
- **Viento:** 15 km/h, dirección 180° (Sur)
- **Índice UV:** 8 (Alto)
```

### Análisis Histórico
```
Usuario: Compara las temperaturas de Santiago entre enero y julio de 2023

Claude: Voy a obtener los datos históricos de temperatura de Santiago para hacer la comparación entre enero y julio de 2023.

[Claude usa get_historical_temperature_monthly con station_code y diferentes meses]

**Análisis de Temperaturas - Santiago 2023:**

**Enero (Verano):**
- Temperatura máxima promedio: 29.8°C
- Temperatura mínima promedio: 14.2°C
- Temperatura media: 22.0°C

**Julio (Invierno):**
- Temperatura máxima promedio: 15.1°C
- Temperatura mínima promedio: 3.7°C
- Temperatura media: 9.4°C

**Diferencia estacional:** 12.6°C en la temperatura media
```

## 📱 Uso Avanzado

### Automatización con Scripts

Puedes crear scripts que usen Claude con WeatherMCP:

```python
# ejemplo_weather.py
import requests
import json

# Claude API call con MCP tools
def get_weather_data(station_code):
    # Tu lógica para llamar a Claude con MCP
    pass

# Uso
weather_data = get_weather_data("330001")
print(json.dumps(weather_data, indent=2))
```

### Integración con Otros Sistemas

```bash
# Crontab ejemplo: reporte diario automatizado
0 8 * * * python /path/to/daily_weather_report.py
```

## 🎉 ¡Listo!

Una vez configurado correctamente, Claude Desktop podrá:

- ✅ Consultar estaciones meteorológicas de Chile
- ✅ Obtener datos meteorológicos en tiempo real
- ✅ Acceder a datos históricos
- ✅ Generar análisis y comparaciones
- ✅ Crear reportes meteorológicos personalizados

¡Ahora tienes acceso completo a los datos de MeteoChile directamente desde Claude! 🌤️