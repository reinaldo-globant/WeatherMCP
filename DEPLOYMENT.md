# WeatherMCP - Guía de Despliegue

Esta guía cubre diferentes opciones de despliegue para WeatherMCP usando .NET 9.

## ✅ Docker (Recomendado)

### Desarrollo Rápido
```bash
# Construir y ejecutar con docker-compose
docker-compose -f docker-compose.dev.yml up -d

# Acceder a Swagger: http://localhost:5175/swagger
```

### Producción
```bash
# Usar imagen optimizada de Alpine Linux
docker build -f Dockerfile.production -t weathermcp:prod .

# Ejecutar en producción
docker-compose up -d
```

### Características Docker

| Característica | Desarrollo | Producción |
|----------------|------------|------------|
| Base Image | `mcr.microsoft.com/dotnet/aspnet:9.0` | `mcr.microsoft.com/dotnet/aspnet:9.0-alpine` |
| Tamaño | ~200MB | ~120MB |
| Health Check | ✅ | ✅ |
| Non-root User | ✅ | ✅ |
| Multi-stage Build | ✅ | ✅ |

## 🚀 Despliegue en la Nube

### Azure Container Instances
```bash
# Construir y subir a ACR
az acr build --registry myregistry --image weathermcp .

# Desplegar
az container create \
  --resource-group myResourceGroup \
  --name weathermcp \
  --image myregistry.azurecr.io/weathermcp:latest \
  --ports 8080 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    MeteoChile__BaseUrl="https://climatologia.meteochile.gob.cl/application/servicios/json/"
```

### Google Cloud Run
```bash
# Construir y subir
gcloud builds submit --tag gcr.io/PROJECT-ID/weathermcp

# Desplegar
gcloud run deploy \
  --image gcr.io/PROJECT-ID/weathermcp \
  --platform managed \
  --port 8080 \
  --set-env-vars="ASPNETCORE_ENVIRONMENT=Production"
```

### AWS ECS/Fargate
```bash
# Construir y subir a ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com

docker build -f Dockerfile.production -t weathermcp .
docker tag weathermcp:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/weathermcp:latest
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/weathermcp:latest
```

## 🔧 Configuración de Producción

### Variables de Entorno Críticas
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
MeteoChile__BaseUrl=https://climatologia.meteochile.gob.cl/application/servicios/json/
MeteoChile__TimeoutSeconds=30
Logging__LogLevel__Default=Information
```

### Seguridad
- ✅ HTTPS en producción (usar proxy reverso)
- ✅ Usuario no privilegiado
- ✅ Secrets management
- ✅ Health checks configurados
- ✅ Logging estructurado

## 📊 Monitoreo

### Health Checks
- **Endpoint**: `GET /health`
- **Respuesta exitosa**: `{"status":"healthy","timestamp":"..."}`
- **Docker**: Health check automático cada 30s

### Métricas Disponibles
- Tiempo de respuesta de la API de MeteoChile
- Estado del servidor MCP
- Número de peticiones procesadas
- Errores de conexión

### Logging
```json
{
  "timestamp": "2024-01-15T10:30:00.000Z",
  "level": "Information",
  "message": "Calling MeteoChile API: https://climatologia.meteochile.gob.cl/...",
  "properties": {
    "SourceContext": "WeatherMCP.Infrastructure.ExternalServices.MeteoChileApiClient",
    "Url": "https://climatologia.meteochile.gob.cl/application/servicios/json/getCatastroEstaciones"
  }
}
```

## 🔍 Troubleshooting

### Problemas Comunes

1. **Error de conexión a MeteoChile API**
   ```bash
   # Verificar conectividad
   docker exec weathermcp curl -f https://climatologia.meteochile.gob.cl/application/servicios/json/getCatastroEstaciones
   ```

2. **Health check fallando**
   ```bash
   # Verificar logs
   docker-compose logs weathermcp
   
   # Health check manual
   docker exec weathermcp ./healthcheck.sh
   ```

3. **Puerto no disponible**
   ```bash
   # Cambiar puerto en docker-compose.yml
   ports:
     - "5176:8080"  # Usar puerto diferente
   ```

## 📈 Escalabilidad

### Load Balancer
```yaml
# nginx.conf ejemplo
upstream weathermcp {
    server weathermcp1:8080;
    server weathermcp2:8080;
    server weathermcp3:8080;
}

server {
    listen 80;
    location / {
        proxy_pass http://weathermcp;
    }
}
```

### Docker Swarm
```bash
# Crear servicio escalable
docker service create \
  --name weathermcp \
  --replicas 3 \
  --publish 5175:8080 \
  weathermcp:prod
```

### Kubernetes
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: weathermcp
spec:
  replicas: 3
  selector:
    matchLabels:
      app: weathermcp
  template:
    metadata:
      labels:
        app: weathermcp
    spec:
      containers:
      - name: weathermcp
        image: weathermcp:prod
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
```