# Contribuir a WeatherMCP

¡Gracias por tu interés en contribuir a WeatherMCP! Este documento te guiará a través del proceso de contribución.

## 🚀 Quick Start

1. **Fork** el repositorio
2. **Clone** tu fork localmente
3. **Crea** una rama para tu feature
4. **Desarrolla** y **testea** tus cambios
5. **Commit** tus cambios
6. **Push** a tu fork
7. **Crea** un Pull Request

## 📋 Antes de Empezar

### Prerrequisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](DOCKER_SETUP.md) (Colima o Docker Desktop)
- Git

### Configuración del Entorno

```bash
# Clonar tu fork
git clone https://github.com/tu-usuario/WeatherMCP.git
cd WeatherMCP

# Configurar remote upstream
git remote add upstream https://github.com/original-usuario/WeatherMCP.git

# Instalar dependencias
dotnet restore

# Verificar que todo funciona
dotnet build
dotnet test
```

## 🏗️ Arquitectura del Proyecto

WeatherMCP sigue los principios de **Clean Architecture**:

```
WeatherMCP/
├── Application/          # Casos de uso y lógica de aplicación
│   ├── Interfaces/      # Contratos
│   └── Services/        # Servicios de dominio
├── Domain/              # Modelos de dominio
│   └── Models/         # Entidades y value objects
├── Infrastructure/      # Implementaciones de infraestructura
│   ├── ExternalServices/ # Clientes de APIs externas
│   └── MCP/            # Protocolo MCP
└── Presentation/        # Controladores y endpoints
    └── Controllers/     # REST API controllers
```

## 💻 Tipos de Contribuciones

### 🐛 Reportar Bugs

Usa el template de issue para bugs:
- Describe el problema claramente
- Incluye pasos para reproducir
- Proporciona información del entorno
- Adjunta logs si es posible

### ✨ Proponer Features

- Describe el caso de uso
- Explica el beneficio esperado
- Considera el impacto en la API existente
- Proporciona mockups si aplica

### 📝 Mejorar Documentación

- README.md
- Comentarios en código
- Guías de setup
- Ejemplos de uso

## 🔧 Desarrollo

### Estándares de Código

#### C# / .NET

```csharp
// ✅ Buenas prácticas
public class WeatherService
{
    private readonly IMeteoChileApiClient _apiClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IMeteoChileApiClient apiClient, ILogger<WeatherService> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WeatherData> GetWeatherAsync(string stationCode)
    {
        _logger.LogInformation("Getting weather for station {StationCode}", stationCode);
        
        try 
        {
            return await _apiClient.GetStationDataAsync(stationCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weather for station {StationCode}", stationCode);
            throw;
        }
    }
}
```

#### Convenciones

- **PascalCase** para clases, métodos y propiedades
- **camelCase** para variables locales y parámetros
- **_camelCase** para fields privados
- **Async/await** para operaciones asíncronas
- **ConfigureAwait(false)** en bibliotecas
- **Logging estructurado** con parámetros tipados

### Testing

```bash
# Ejecutar tests unitarios
dotnet test

# Con coverage
dotnet test --collect:"XPlat Code Coverage"

# Tests específicos
dotnet test --filter "FullyQualifiedName~WeatherServiceTests"
```

### Docker

```bash
# Desarrollo
docker-compose -f docker-compose.dev.yml up -d

# Producción
docker-compose up -d

# Rebuild
docker-compose build --no-cache
```

## 📝 Commit Messages

Usa el formato [Conventional Commits](https://www.conventionalcommits.org/):

```
tipo(alcance): descripción

[cuerpo opcional]

[pie opcional]
```

### Tipos

- `feat`: Nueva funcionalidad
- `fix`: Corrección de bug
- `docs`: Documentación
- `style`: Formato (sin cambios de código)
- `refactor`: Refactoring
- `test`: Tests
- `chore`: Tareas de mantenimiento

### Ejemplos

```
feat(api): add historical pressure endpoints

Add new REST endpoints for historical pressure data:
- GET /api/historical/pressure/monthly/{code}
- GET /api/historical/pressure/daily/{code}/{year}

Closes #123
```

```
fix(docker): update Dockerfile to use .NET 9

- Update base image to mcr.microsoft.com/dotnet/aspnet:9.0
- Fix package references for .NET 9 compatibility
- Add health check configuration

Fixes #456
```

## 🔍 Pull Request Process

### Antes del PR

1. **Sync** con upstream:
   ```bash
   git fetch upstream
   git checkout main
   git merge upstream/main
   ```

2. **Rebase** tu feature branch:
   ```bash
   git checkout feature/mi-feature
   git rebase main
   ```

3. **Tests y build**:
   ```bash
   dotnet build --configuration Release
   dotnet test
   ```

### Template de PR

```markdown
## Descripción

Descripción clara y concisa de los cambios.

## Tipo de cambio

- [ ] Bug fix (cambio que no rompe funcionalidad existente)
- [ ] New feature (cambio que añade funcionalidad)
- [ ] Breaking change (cambio que rompería funcionalidad existente)
- [ ] Documentation update

## Testing

- [ ] Tests unitarios pasan
- [ ] Tests de integración pasan
- [ ] Build de producción exitoso
- [ ] Docker build exitoso

## Checklist

- [ ] Código sigue las convenciones del proyecto
- [ ] Auto-review realizado
- [ ] Documentación actualizada
- [ ] Tests añadidos/actualizados
- [ ] No hay conflictos de merge
```

### Review Process

1. **Automated checks** deben pasar
2. **Code review** por al menos 1 maintainer
3. **Tests** ejecutados automáticamente
4. **Merge** por maintainer

## 📚 Recursos Útiles

### Documentación

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Docker Best Practices](https://docs.docker.com/develop/best-practices/)

### Tools

- [Visual Studio Code](https://code.visualstudio.com/)
- [.NET CLI](https://docs.microsoft.com/dotnet/core/tools/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) / [Colima](https://github.com/abiosoft/colima)

## ❓ Preguntas

Si tienes preguntas:

1. Revisa la [documentación existente](README.md)
2. Busca en [issues existentes](../../issues)
3. Crea un [nuevo issue](../../issues/new) con la tag `question`

## 📄 Licencia

Al contribuir, aceptas que tus contribuciones serán licenciadas bajo la misma licencia que el proyecto.

---

¡Gracias por contribuir a WeatherMCP! 🙏