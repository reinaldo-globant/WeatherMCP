# Docker Setup Guide - WeatherMCP

Esta guía te ayudará a configurar Docker en tu sistema para ejecutar WeatherMCP.

## 🐳 Opciones de Docker

### Opción 1: Docker Desktop (Más Fácil)

**Instalación en macOS:**
1. Descarga Docker Desktop desde: https://www.docker.com/products/docker-desktop/
2. Instala la aplicación
3. Inicia Docker Desktop desde Launchpad
4. Verifica la instalación:
   ```bash
   docker --version
   docker-compose --version
   ```

**Instalación en Windows:**
1. Descarga Docker Desktop para Windows
2. Instala con WSL2 habilitado
3. Reinicia el sistema
4. Verifica la instalación en PowerShell

### Opción 2: Colima (macOS/Linux - Ligero y Rápido)

**¿Qué es Colima?**
Colima es una alternativa ligera a Docker Desktop que usa Lima-VM para ejecutar contenedores Docker.

**Instalación en macOS:**
```bash
# Instalar con Homebrew
brew install colima docker docker-compose

# O instalar manualmente
curl -LO https://github.com/abiosoft/colima/releases/latest/download/colima-$(uname -s)-$(uname -m)
chmod +x colima-*
sudo mv colima-* /usr/local/bin/colima
```

**Configuración de Colima:**
```bash
# Iniciar Colima (primera vez)
colima start --cpu 4 --memory 8 --disk 50

# Verificar estado
colima status

# Configurar Docker context (si es necesario)
docker context use colima
```

**Instalación en Linux:**
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Instalar Colima
wget -O colima https://github.com/abiosoft/colima/releases/latest/download/colima-Linux-x86_64
chmod +x colima && sudo mv colima /usr/local/bin/
```

## ⚡ Configuración Optimizada

### Configuración de Colima para WeatherMCP

```bash
# Configuración recomendada para desarrollo
colima start \
  --cpu 4 \
  --memory 8 \
  --disk 50 \
  --vm-type=qemu \
  --mount-type=sshfs \
  --dns=1.1.1.1

# Para producción
colima start \
  --cpu 2 \
  --memory 4 \
  --disk 20 \
  --vm-type=qemu
```

### Configuración de Docker Desktop

**Recursos recomendados:**
- **CPU**: 4 cores
- **Memory**: 8 GB
- **Disk**: 50 GB
- **Swap**: 2 GB

**Configurar en Docker Desktop:**
1. Abre Docker Desktop
2. Ve a Settings (⚙️)
3. Selecciona "Resources"
4. Ajusta CPU, Memory y Disk
5. Aplica y reinicia

## 🚀 Ejecutar WeatherMCP

### Opción A: Docker Compose (Recomendado)

```bash
# Clonar el repositorio
git clone https://github.com/tu-usuario/WeatherMCP.git
cd WeatherMCP

# Desarrollo
docker-compose -f docker-compose.dev.yml up -d

# Producción
docker-compose up -d

# Ver logs
docker-compose logs -f

# Acceder a la aplicación
# http://localhost:5175/swagger
```

### Opción B: Docker Build Manual

```bash
# Construir imagen de desarrollo
docker build -t weathermcp:dev .

# Construir imagen de producción (Alpine)
docker build -f Dockerfile.production -t weathermcp:prod .

# Ejecutar contenedor
docker run -d \
  --name weathermcp \
  -p 5175:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  weathermcp:prod
```

## 🔧 Troubleshooting

### Problemas Comunes con Colima

**1. Colima no inicia:**
```bash
# Eliminar y recrear
colima delete
colima start --cpu 4 --memory 8

# Verificar logs
colima logs
```

**2. Docker context incorrecto:**
```bash
# Listar contextos
docker context ls

# Cambiar a colima
docker context use colima

# O usar default
docker context use default
```

**3. Problemas de red:**
```bash
# Reiniciar Colima con DNS personalizado
colima stop
colima start --dns=8.8.8.8,1.1.1.1
```

**4. Problemas de montaje de volúmenes:**
```bash
# Usar sshfs para mejor rendimiento
colima start --mount-type=sshfs
```

### Problemas Comunes con Docker Desktop

**1. Docker Desktop no inicia:**
- Reinicia la aplicación
- Verifica que tengas suficiente memoria
- Resetea a configuración de fábrica si es necesario

**2. Contenedor no puede acceder a internet:**
- Verifica configuración de DNS en Settings
- Reinicia Docker Desktop
- Verifica firewall/antivirus

**3. Rendimiento lento:**
- Aumenta recursos asignados
- Habilita "Use gRPC FUSE for file sharing"
- Usa bind mounts en lugar de volúmenes para desarrollo

## ⚡ Comandos Útiles

### Colima
```bash
# Gestión básica
colima start                    # Iniciar
colima stop                     # Detener  
colima restart                  # Reiniciar
colima delete                   # Eliminar VM
colima status                   # Ver estado
colima list                     # Listar instancias

# Información del sistema
colima version                  # Versión
colima ssh                      # SSH a la VM
```

### Docker General
```bash
# Información del sistema
docker info                     # Info del sistema
docker system df                # Uso de disco
docker system prune             # Limpiar recursos no usados

# Contenedores
docker ps                       # Contenedores corriendo
docker ps -a                    # Todos los contenedores
docker logs weathermcp          # Ver logs
docker exec -it weathermcp bash # Acceder al contenedor

# Imágenes
docker images                   # Listar imágenes
docker rmi weathermcp:dev      # Eliminar imagen
```

## 📊 Comparación Colima vs Docker Desktop

| Característica | Colima | Docker Desktop |
|----------------|--------|----------------|
| **Tamaño** | ~50MB | ~500MB |
| **RAM en reposo** | ~200MB | ~1GB |
| **CPU en reposo** | ~1% | ~5% |
| **Velocidad de inicio** | ~30s | ~60s |
| **GUI** | ❌ | ✅ |
| **Kubernetes** | ✅ (manual) | ✅ (integrado) |
| **Costo** | Gratis | Gratis/Comercial |
| **Plataformas** | macOS, Linux | macOS, Windows, Linux |

## 🎯 Recomendaciones

### Para Desarrollo:
- **Mac con recursos limitados**: Colima
- **Mac con recursos abundantes**: Docker Desktop
- **Linux**: Colima
- **Windows**: Docker Desktop

### Para CI/CD:
- **GitHub Actions**: Docker preinstalado
- **GitLab CI**: Usar imagen docker:dind
- **Jenkins**: Plugin de Docker

### Para Producción:
- **Cloud**: Servicios nativos (ECS, Cloud Run, AKS)
- **On-premise**: Docker Swarm o Kubernetes
- **Edge**: Docker Compose con restart policies

## 🚀 Quick Start

```bash
# 1. Instalar Docker (elige una opción)
brew install colima docker docker-compose    # Colima
# O instalar Docker Desktop

# 2. Iniciar Docker
colima start --cpu 4 --memory 8             # Colima
# O iniciar Docker Desktop

# 3. Verificar instalación
docker --version && docker-compose --version

# 4. Clonar y ejecutar WeatherMCP
git clone https://github.com/tu-usuario/WeatherMCP.git
cd WeatherMCP
docker-compose up -d

# 5. Acceder a la aplicación
open http://localhost:5175/swagger
```

¡Listo! WeatherMCP debería estar ejecutándose en tu sistema. 🎉