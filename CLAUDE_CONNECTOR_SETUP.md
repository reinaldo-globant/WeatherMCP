# Claude Custom Connector - WeatherMCP

Esta guía te ayudará a configurar WeatherMCP como un **Custom Connector** en Claude para acceder a datos meteorológicos de Chile.

## 🚀 Paso 1: Ejecutar WeatherMCP Localmente

### Opción A: Con .NET
```bash
# Clonar y ejecutar
git clone https://github.com/tu-usuario/WeatherMCP.git
cd WeatherMCP
dotnet run

# Server will be available at http://localhost:5175
```

### Opción B: Con Docker
```bash
# Usando Docker Compose
docker-compose up -d

# Verificar que funciona
curl http://localhost:5175/health
```

### Verificar Swagger UI
Accede a **http://localhost:5175/swagger** para ver todos los endpoints disponibles.

## 🔌 Paso 2: Configurar Custom Connector en Claude

### En Claude Desktop/Web:

1. **Ir a Settings/Configuración**
2. **Seleccionar "Custom Connectors"**
3. **Hacer clic en "Add Connector"**
4. **Subir o pegar la configuración:**

Usa el archivo `claude-connector-config.json` del repositorio, o copia esta configuración:

```json
{
  "name": "WeatherMCP Chile",
  "description": "Acceso a datos meteorológicos de Chile desde MeteoChile",
  "base_url": "http://localhost:5175/api",
  "endpoints": [
    {
      "name": "get_weather_stations",
      "description": "Obtiene el catastro de estaciones meteorológicas de Chile",
      "method": "GET",
      "path": "/weather/stations",
      "parameters": []
    },
    {
      "name": "get_station_recent_data",
      "description": "Obtiene datos meteorológicos recientes de una estación específica",
      "method": "GET",
      "path": "/weather/stations/{station_code}/recent-data",
      "parameters": [
        {
          "name": "station_code",
          "type": "string",
          "required": true,
          "description": "Código de la estación (ej: 330001 para Arica)"
        }
      ]
    },
    {
      "name": "get_historical_temperature_daily",
      "description": "Obtiene datos históricos de temperatura diaria",
      "method": "GET",
      "path": "/historical/temperature/daily/{station_code}/{year}",
      "parameters": [
        {
          "name": "station_code",
          "type": "string",
          "required": true,
          "description": "Código de la estación meteorológica"
        },
        {
          "name": "year",
          "type": "integer",
          "required": true,
          "description": "Año (ej: 2024)"
        }
      ]
    }
  ],
  "authentication": {
    "type": "none"
  }
}
```

### Configuración Completa

Para acceso a **todos** los endpoints, usa el archivo completo `claude-connector-config.json` del repositorio.

## 🧪 Paso 3: Probar el Connector

Una vez configurado, puedes hacer preguntas como:

### Consultas Básicas
```
¿Qué estaciones meteorológicas hay en Chile?
```

```
¿Cuál es el clima actual en Arica?
```

```
Muéstrame los datos recientes de todas las estaciones del norte de Chile
```

### Consultas Avanzadas
```
Compara las temperaturas históricas de Santiago entre enero y julio de 2023
```

```
¿Cuál ha sido la tendencia de precipitaciones en Valparaíso en los últimos 5 años?
```

```
Necesito un reporte del índice UV para planificar actividades al aire libre
```

## 📊 Endpoints Disponibles

| Función | Endpoint | Descripción |
|---------|----------|-------------|
| **Catastro** | `/weather/stations` | Todas las estaciones |
| **Datos Actuales** | `/weather/stations/{code}/recent-data` | Clima actual |
| **Datos Mensuales** | `/weather/stations/{code}/monthly-data/{year}/{month}` | Datos detallados |
| **Resumen Diario** | `/weather/stations/daily-summary` | Resumen de todas las estaciones |
| **Índice UV** | `/weather/uv-index` | Radiación ultravioleta |
| **Boletín** | `/weather/climatological-bulletin` | Boletín oficial |
| **Historia Temperatura** | `/historical/temperature/daily/{code}/{year}` | Temperaturas históricas |
| **Historia Precipitación** | `/historical/precipitation/daily/{code}/{year}` | Precipitaciones históricas |
| **Historia Presión** | `/historical/pressure/daily/{code}/{year}` | Presión atmosférica |

## 🎯 Códigos de Estaciones Principales

| Estación | Código | Región |
|----------|--------|---------|
| **Arica** | 330001 | XV - Arica y Parinacota |
| **Iquique** | 330002 | I - Tarapacá |
| **Calama** | 330003 | II - Antofagasta |
| **Copiapó** | 330019 | III - Atacama |
| **La Serena** | 330030 | IV - Coquimbo |
| **Santiago** | 330007 | RM - Metropolitana |
| **Valparaíso** | 330008 | V - Valparaíso |
| **Concepción** | 330013 | VIII - Biobío |
| **Temuco** | 330014 | IX - La Araucanía |
| **Valdivia** | 330015 | XIV - Los Ríos |
| **Puerto Montt** | 330016 | X - Los Lagos |
| **Punta Arenas** | 330017 | XII - Magallanes |

## 🔧 Troubleshooting

### Problema: "Cannot connect to localhost"

**Solución:**
```bash
# Verificar que el servidor esté ejecutándose
curl http://localhost:5175/health

# Si no responde, iniciar el servidor
dotnet run
# o
docker-compose up -d
```

### Problema: "Endpoint not found"

**Solución:**
- Verificar que la URL base sea `http://localhost:5175/api`
- Revisar que los paths en el connector coincidan con los del Swagger
- Acceder a http://localhost:5175/swagger para ver endpoints disponibles

### Problema: "Authentication failed"

**Solución:**
- WeatherMCP no requiere autenticación
- Configurar `authentication: { "type": "none" }` en el connector

### Problema: Datos vacíos o errores

**Solución:**
```bash
# Probar endpoints manualmente
curl http://localhost:5175/api/weather/stations
curl http://localhost:5175/api/weather/stations/330001/recent-data

# Revisar logs del servidor
docker-compose logs weathermcp
# o ver logs en la consola donde ejecutaste dotnet run
```

## 🌐 Despliegue en Producción

Para usar en producción, necesitas desplegar WeatherMCP en un servidor accesible:

### Con Railway/Heroku/etc:
1. Desplegar WeatherMCP en la plataforma
2. Obtener la URL pública (ej: `https://tu-app.railway.app`)
3. Actualizar `base_url` en el connector config:
   ```json
   {
     "base_url": "https://tu-app.railway.app/api",
     ...
   }
   ```

### Con ngrok (desarrollo):
```bash
# En una terminal
dotnet run

# En otra terminal
ngrok http 5175

# Usar la URL de ngrok en el connector
# ej: "base_url": "https://abc123.ngrok.io/api"
```

## 📱 Ejemplos de Uso

### Consulta Simple
```
Usuario: ¿Cuál es la temperatura actual en Santiago?

Claude: [Usa get_station_recent_data con station_code="330007"]

La temperatura actual en Santiago es de 18.5°C con una humedad del 65% y vientos de 12 km/h del suroeste.
```

### Análisis Histórico
```
Usuario: Compara las precipitaciones de enero 2023 vs enero 2024 en Valparaíso

Claude: [Usa get_historical_precipitation_daily para ambos años]

**Precipitaciones Enero - Valparaíso:**

**2023:**
- Total mensual: 2.4mm
- Días con lluvia: 3
- Precipitación máxima: 1.8mm (15 enero)

**2024:**
- Total mensual: 15.6mm
- Días con lluvia: 8
- Precipitación máxima: 4.2mm (22 enero)

**Conclusión:** Enero 2024 fue significativamente más lluvioso (+550%) comparado con 2023.
```

### Reporte Integral
```
Usuario: Necesito un reporte meteorológico completo para planificar un evento en Arica

Claude: [Usa múltiples endpoints: recent_data, daily_summary, uv_index]

**Reporte Meteorológico - Arica**

**Condiciones Actuales:**
- Temperatura: 24.8°C
- Humedad: 58%
- Viento: 15 km/h (NO)
- Presión: 1015.2 hPa

**Pronóstico Diario:**
- Máxima: 28°C
- Mínima: 19°C
- Probabilidad lluvia: 0%

**Índice UV:** 9 (Muy Alto)
- Recomendación: Usar protector solar factor 50+

**Ideal para eventos al aire libre** ☀️
```

## 🎉 ¡Listo!

Con esta configuración, Claude puede:

- ✅ Consultar datos meteorológicos en tiempo real
- ✅ Acceder a información histórica
- ✅ Generar análisis comparativos
- ✅ Crear reportes personalizados
- ✅ Responder preguntas específicas sobre el clima en Chile

¡Ahora tienes acceso completo a los datos de MeteoChile directamente desde Claude! 🌤️