# Gestión de Salas

Aplicación web ASP.NET Core MVC (NET 9) para administrar salas, equipos y préstamos dentro de una institución. El repositorio implementa una arquitectura por capas (Domain → Infrastructure → Services → Web) más una batería de pruebas unitarias y de integración para cada capa.

## Tabla de contenido
1. Resumen
2. Arquitectura por proyectos
3. Funcionalidades principales
4. Modelo de datos
5. Dependencias destacadas
6. Configuración y ejecución local
7. Migraciones y base de datos
8. Pruebas automatizadas
9. Estructura de carpetas
10. Estándares y notas

## 1. Resumen
- **Objetivo:** controlar la asignación de salas y equipos institucionales, gestionar préstamos, reportes de daño y asesorías entre usuarios, coordinadores y administradores.
- **Patrón:** arquitectura limpia con separación de responsabilidades, repositorios EF Core y servicios de dominio orquestados desde un frontend MVC tradicional.
- **Autenticación:** sesiones ASP.NET Core con hashing de contraseñas mediante `IPasswordHasher<Usuario>` y filtro personalizado `AuthorizeRoleAttribute` basado en roles (`Administrador`, `Coordinador`, `Usuario`).

## 2. Arquitectura por proyectos

| Proyecto / Ruta | Tipo | Responsabilidad principal |
| --- | --- | --- |
| `Domain/Domain` | Class Library | Entidades, enums, validaciones con `DataAnnotations`. |
| `Infrastructure/Infrastructure` | Class Library | `AppDbContext`, migraciones EF Core, repositorios (User, Sala, Equipo, Solicitud, Reporte, Asesoría). |
| `Services/Services` | Class Library | Lógica de negocio, mapeos `AutoMapper`, hashing de credenciales, reglas de validación. |
| `Web/MvcSample` | ASP.NET Core MVC | UI MVC, controladores por rol, filtros, vistas Razor, assets estáticos, configuración de DI. |
| `Test/*` | Class Libraries | Pruebas unitarias y de integración por capa (`DomainTest`, `InfrastructureTest`, `ServicesTest`). |

Las referencias siguen el flujo `Web → Services → Infrastructure → Domain`. El archivo de solución principal es `Web/MvcSample/MvcSample.sln`.

## 3. Funcionalidades principales
- **Panel de administrador:** tablero con métricas de salas, equipos y usuarios; ABM completo de salas, equipos y usuarios (`AdminController`, `SalaController`, `UsuarioController`, etc.).
- **Gestión de salas y equipos:** creación, edición, asignación de responsables y control de estados (`EstadoSala`, `EstadoEquipo`, bloqueos con prioridad y motivo).
- **Préstamos de salas/equipos:** usuarios registran solicitudes, coordinadores o administradores aprueban/rechazan, se lleva trazabilidad de tiempos estimados y observaciones (`SolicitudPrestamoService` y `SolicitudPrestamoController` en la UI).
- **Reportes de daño y mantenimiento:** cualquier usuario reporta daños sobre equipos o salas, los coordinadores resuelven y documentan prioridades (`ReporteDanoService`).
- **Asesorías:** canal para que usuarios pidan soporte especializado; cada asesoría pasa por estados (`EstadoAsesoria`) y puede asignarse a coordinadores.
- **Seguridad:** control de acceso en los controladores mediante `AuthorizeRoleAttribute`, sesiones persistentes y gestión de contraseñas hash.
- **Front-end MVC:** vistas Razor en `Web/MvcSample/Views`, assets en `wwwroot` (CSS, JS, Bootstrap, íconos). Incluye `DinkToPdf` para generación de reportes/constancias en PDF cuando se requiera.

## 4. Modelo de datos
- `Usuario`: datos personales, credenciales hash, rol y bitácoras de actividad; navegación hacia salas, equipos, solicitudes, reportes y asesorías.
- `Sala`: número, capacidad, ubicación, estado y responsable asignado; relación 1:N con `Equipo`.
- `Equipo`: pertenece a una sala, puede asignarse a un usuario y mantenerse bloqueado con motivo, prioridad y fecha.
- `SolicitudPrestamo`: relación entre usuario, sala y opcionalmente equipo; almacena tiempos, estado, motivo de rechazo y aprobador.
- `ReporteDano`: describe incidentes sobre equipos o salas con prioridad, estado y resolutor.
- `Asesoria`: solicitudes de orientación con priorización, cronología e identificador del coordinador que atiende.
- `Enums` (`EstadoSala`, `RolUsuario`, `EstadoEquipo`, `EstadoSolicitud`, `EstadoReporte`, `PrioridadReporte`, `TipoReporte`, `EstadoAsesoria`, `TipoSolicitudPrestamo`) estandarizan los estados y transiciones usados en toda la solución.

## 5. Dependencias destacadas
- **.NET 9 SDK** para todos los proyectos.
- **Entity Framework Core 9.0.10** (`SqlServer`, `Tools`, `Design`) y `Microsoft.Extensions.*` para configuración/DI.
- **AutoMapper 13** para mapear entidades a modelos de vista.
- **Microsoft.AspNetCore.Identity** para hashing de contraseñas sin depender de Identity UI completo.
- **DinkToPdf 1.0.8** integrado en el proyecto MVC para exportaciones.

## 6. Configuración y ejecución local
1. **Requisitos previos:** .NET 9 SDK, SQL Server (local o remoto) y acceso a PowerShell o Bash.
2. **Clonar:** `git clone <repo>` y `cd Gestion-Salas`.
3. **Restaurar paquetes:** `dotnet restore Web/MvcSample/MvcSample.sln`.
4. **Configurar conexión a base:** editar `Web/MvcSample/appsettings.Development.json` (o usar `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena>"`). La cadena por defecto apunta a SQL Server remoto; reemplázala por una instancia propia para desarrollo.
5. **Variables opcionales:** `ASPNETCORE_ENVIRONMENT=Development` para habilitar páginas de error detalladas y `UseMigrationsEndPoint`.
6. **Compilar:** `dotnet build Web/MvcSample/MvcSample.sln`.
7. **Ejecutar:** `dotnet run --project Web/MvcSample/MvcSample.csproj`. La aplicación quedará disponible en `https://localhost:5001` (o el puerto indicado por Kestrel).

> Consejo: si vas a publicar en IIS/Azure, utiliza `dotnet publish Web/MvcSample/MvcSample.csproj -c Release -o ./publish`.

## 7. Migraciones y base de datos
1. Instala la herramienta EF si aún no la tienes: `dotnet tool install --global dotnet-ef`.
2. Crea o actualiza migraciones desde la raíz:
   ```bash
   dotnet ef migrations add NombreMigracion \
     --project Infrastructure/Infrastructure/Infrastructure.csproj \
     --startup-project Web/MvcSample/MvcSample.csproj
   ```
3. Aplica migraciones:
   ```bash
   dotnet ef database update \
     --project Infrastructure/Infrastructure/Infrastructure.csproj \
     --startup-project Web/MvcSample/MvcSample.csproj
   ```
4. Para entornos CI/CD, configura la cadena `DefaultConnection` en variables de entorno/secretos y ejecuta `dotnet ef database update` antes de arrancar el sitio.

## 8. Pruebas automatizadas
- Cada capa tiene su proyecto de pruebas (`Test/DomainTest`, `Test/InfrastructureTest`, `Test/ServicesTest`), alineado a los servicios y repositorios correspondientes.
- Ejecuta todas las pruebas con:
  ```bash
  dotnet test Web/MvcSample/MvcSample.sln
  ```
- Las pruebas de infraestructura utilizan un `InMemoryDbContextFactory` para aislar escenarios de EF Core sin necesidad de SQL Server.

## 9. Estructura de carpetas
```
Gestion-Salas/
├── Domain/Domain/              # Entidades, enums y validaciones
├── Infrastructure/Infrastructure/
│   ├── AppDbContext.cs         # DbContext EF Core
│   ├── Repositories/           # Repositorios específicos
│   └── Migrations/             # Historial de migraciones
├── Services/Services/          # Reglas de negocio, AutoMapper, modelos de vista
├── Web/MvcSample/              # Proyecto web MVC, controladores, vistas, wwwroot
├── Test/                       # Pruebas por capa
└── Scripts/                    # (Reservado para automatizaciones futuras)
```

## 10. Estándares y notas
- **Sesiones y seguridad:** siempre inicializa la sesión antes de restringir acceso; los roles se leen desde `HttpContext.Session` (ver `AuthorizeRoleAttribute`).
- **Contraseñas:** usa `IUserService.Register` o `ChangePasswordAsync` para garantizar hashing con `PasswordHasher<Usuario>`.
- **AutoMapper:** las proyecciones se definen en `Services/Services/AutoMapper/MappingProfile.cs`; agrega nuevos modelos allí antes de consumirlos en vistas.
- **Estilo de código:** se sigue `ImplicitUsings` y `nullable` habilitado; evita introducir dependencias circulares entre proyectos.
- **Configuraciones sensibles:** no dejes credenciales en `appsettings.json`; migra a `appsettings.Development.json` o Secret Manager.

---
