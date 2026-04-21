# AGENTS.md — Guía para Agentes de Código

> Este archivo describe la arquitectura, convenciones y comandos esenciales del proyecto **UnimagEmprende / Sistema de Gestión de Eventos**. Está escrito en español porque todo el código fuente, comentarios y documentación del proyecto usan español como idioma principal.

---

## 1. Visión General del Proyecto

Sistema de gestión de eventos construido como API REST con **.NET 8** y arquitectura en capas (Domain → Application → Infrastructure → API). El alcance actual cubre el núcleo de identidad: registro de usuarios, roles (Admin/User) y persistencia en PostgreSQL. La autenticación JWT está configurada a nivel de infraestructura, pero el endpoint de login aún no está implementado.

**Stack tecnológico clave:**
- **Backend:** C# 12, .NET 8 (SDK mínimo 8.0)
- **Base de datos:** PostgreSQL 16, EF Core 8.0.0
- **Validación:** FluentValidation.AspNetCore 11.3.0
- **Hashing de contraseñas:** BCrypt.Net-Next 4.0.3
- **Contenedores:** Docker 24+, Docker Compose v2
- **Infraestructura objetivo:** Azure (planeado)
- **CI/CD:** GitHub Actions (planeado, aún no hay workflows en `.github/`)

---

## 2. Estructura de la Solución

```
EventManager.sln
├── EventManager.Domain/           → Entidades, enums, excepciones (sin dependencias externas)
├── EventManager.Application/      → DTOs, interfaces de repositorio/servicio, lógica de negocio
├── EventManager.Infrastructure/   → EF Core (DbContext, migraciones, configuraciones), repositorios, seguridad
└── EventManager.API/              → Controllers, validadores FluentValidation, punto de entrada HTTP
```

### Reglas de dependencia entre capas

| Capa | Depende de |
|------|-----------|
| `Domain` | Ninguna |
| `Application` | `Domain` |
| `Infrastructure` | `Domain`, `Application` |
| `API` | `Application`, `Infrastructure` *(nota: aunque el `.csproj` comenta "nunca Infrastructure directamente", el proyecto sí la referencia para invocar `AddInfrastructure` en `Program.cs`)* |

### Convenciones de nombres en directorios
- Entidades: `EventManager.Domain/Entities/`
- Configuraciones EF: `EventManager.Infrastructure/Persistence/Configurations/`
- Repositorios: `EventManager.Infrastructure/Persistence/Repositories/`
- Seeders: `EventManager.Infrastructure/Persistence/SeedData/`
- DTOs: `EventManager.Application/DTOs/<Dominio>/`
- Interfaces: `EventManager.Application/Interfaces/`
- Servicios: `EventManager.Application/Services/`
- Controladores: `EventManager.API/Controllers/`
- Validadores: `EventManager.API/Validators/`

---

## 3. Comandos de Construcción y Ejecución

### Requisitos previos
- .NET SDK 8.0+
- Docker 24+ y Docker Compose v2
- Git 2.44+

### Variables de entorno
Copiar `.env.example` a `.env` y completar los valores reales antes de arrancar:

```bash
cp .env.example .env
```

Variables obligatorias:
- `DATABASE_URL` — Cadena Npgsql (ej: `Host=db;Port=5432;Database=eventmanager;Username=postgres;Password=postgres`)
- `JWT__Secret` — Mínimo 32 caracteres
- `JWT__Issuer`, `JWT__Audience`, `JWT__ExpiryMinutes`
- `ASPNETCORE_ENVIRONMENT` — `Development` o `Production`
- `POSTGRES_USER`, `POSTGRES_PASSWORD` — Usadas por el servicio `db` de Docker Compose

### Levantar todo con Docker (recomendado para desarrollo)

```bash
docker compose up -d --build
```

Servicios expuestos:
- API: http://localhost:5000
- pgAdmin: http://localhost:5050 (email: `admin@eventmanager.local`, pass: `admin`)
- PostgreSQL: localhost:5432

### Verificar salud de la API

```bash
curl http://localhost:5000/health
# Respuesta esperada: {"status":"healthy","timestamp":"..."}
```

### Ejecutar migraciones manualmente

```bash
dotnet ef database update \
  --project EventManager.Infrastructure \
  --startup-project EventManager.API
```

> **Nota importante:** En `Program.cs` las migraciones se aplican automáticamente al arrancar (`db.Database.MigrateAsync()`). En producción se recomienda ejecutarlas explícitamente como paso de despliegue, no durante el startup.

### Desarrollo local sin Docker

```bash
# Asegurarse de que PostgreSQL esté corriendo y el archivo .env esté configurado
dotnet run --project EventManager.API
```

Perfiles de launch (ver `Properties/launchSettings.json`):
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001 + http://localhost:5000

### Compilar la solución completa

```bash
dotnet build EventManager.sln
```

### Publicar (modo Release)

```bash
dotnet publish EventManager.API -c Release -o ./publish
```

---

## 4. Guía de Estilo y Convenciones de Código

### Configuración del proyecto
- `Nullable` = `enable` en todos los proyectos.
- `ImplicitUsings` = `enable` en todos los proyectos.
- `sealed` por defecto en clases que no están diseñadas para herencia (ej: `AuthService`, `AuthController`, `RegisterRequestValidator`).

### Idioma
- **Todo el código, comentarios XML, mensajes de excepción y documentación se escriben en español.**
- Ejemplo: `El email '{email}' ya está registrado.`, `La contraseña debe tener al menos 8 caracteres.`

### Nomenclatura
- Clases/records: `PascalCase`
- Métodos y propiedades: `PascalCase`
- Variables locales y parámetros: `camelCase`
- Constantes: `PascalCase` o `UPPER_SNAKE_CASE` según contexto
- Nombres de tablas y columnas en BD: `snake_case` (configurado explícitamente en las `IEntityTypeConfiguration`)

### Entity Framework
- Se usan **configuraciones por entidad** (`IEntityTypeConfiguration<T>`) en lugar de anotaciones de datos.
- Las colecciones de navegación se inicializan vacías: `new List<T>()`.
- Las claves foráneas de unión (many-to-many) usan clave compuesta (ej: `UserRole` con `{ UserId, RoleId }`).
- `AsNoTracking()` en consultas de solo lectura (repositorios).

### Controladores
- Atributo `[Route("api/<recurso>")]` en cada controller.
- Documentar códigos de respuesta con `[ProducesResponseType]`.
- Manejo de excepciones de dominio en el controller (ej: `EmailAlreadyExistsException` → `409 Conflict`).

---

## 5. Instrucciones de Pruebas

**Actualmente no hay proyectos de pruebas unitarias ni de integración en la solución.**

Cuando se agreguen, la convención esperada es:
- Usar `xUnit` (estándar en el ecosistema .NET).
- Nombre del proyecto de pruebas: `EventManager.<Capa>.Tests` o `EventManager.Tests`.
- Colocarlos en la raíz de la solución y referenciarlos en `EventManager.sln`.

### Pruebas manuales actuales
El endpoint de registro puede probarse con:

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "juan.perez@example.com",
    "password": "Password123"
  }'
```

---

## 6. Consideraciones de Seguridad

- **Contraseñas:** Hasheadas con **BCrypt** (factor de trabajo por defecto de la librería). Nunca se almacenan en texto plano.
- **JWT:** La firma requiere `JWT__Secret` de al menos 32 caracteres. Las propiedades `Issuer`, `Audience` y `ExpiryMinutes` son obligatorias.
- **Validación de entrada:** FluentValidation actúa como primera línea de defensa antes de que la solicitud llegue al controller.
- **Excepciones:** En producción, los errores no controlados devuelven un mensaje genérico (`Ocurrió un error interno. Intente más tarde.`) para no filtrar detalles del stack.
- **Variables de entorno:** El archivo `.env` está en `.gitignore`. Nunca commitear secretos.
- **HTTPS:** El pipeline usa `app.UseHttpsRedirection()`; en desarrollo se puede desactivar si es necesario.

---

## 7. Despliegue

### Docker (actual)
El `Dockerfile` usa multi-stage build:
1. **build:** `mcr.microsoft.com/dotnet/sdk:8.0` — restaura dependencias y publica en Release.
2. **runtime:** `mcr.microsoft.com/dotnet/aspnet:8.0` — imagen mínima con el artefacto publicado.

Expone los puertos `80` y `443`.

### Azure (planeado)
El README menciona Azure como entorno de despliegue objetivo, pero no hay archivos de infraestructura como código (Bicep, ARM, Terraform) ni pipelines de GitHub Actions todavía.

---

## 8. Metodología de Control de Versiones

El proyecto sigue **GitFlow**:
- `main` — versión estable/productiva.
- `develop` — integración de funcionalidades terminadas.
- `feature/<nombre>` — una rama por cada Product Backlog Item (PBI).

Ejemplo de historial de commits:
```
15efc6f PBI3
fb73fb3 Merge pull request #19 from keyles-Py/feature/users
7b087b5 Add user and role management entities with configurations and seeding
6cd4c81 Merge pull request #18 from keyles-Py/feature/base-setup
```

---

## 9. Notas para el Agente

- **No asumas la existencia de endpoints de login o recuperación de contraseña.** Solo existe `POST /api/auth/register`.
- **No asumas que hay tests automatizados.** Cualquier cambio debe verificarse con compilación (`dotnet build`) y pruebas manuales contra el endpoint.
- **Mantén el español** en comentarios, mensajes de excepción y documentación XML.
- **Respetar la arquitectura en capas:** la lógica de negocio vive en `Application`, la persistencia en `Infrastructure`, y las entidades puras en `Domain`.
- **Las migraciones de EF Core** deben generarse desde la capa `Infrastructure` con `--startup-project EventManager.API`.
- **El seeder de roles** (`RoleSeeder`) se ejecuta en cada arranque; está diseñado para ser idempotente (`AnyAsync` antes de insertar).
