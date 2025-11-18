using Microsoft.AspNetCore.Mvc;
using Services;
using Domain.Enums;
using MvcSample.Filters;
using System;
using System.Linq;

namespace MvcSample.Controllers
{
    [AuthorizeRole(RolUsuario.Usuario)]
    public class UserController : Controller
    {
        private readonly ISalaService _salaService;
        private readonly IEquipoService _equipoService;
        private readonly ISolicitudPrestamoService _solicitudService;
        private readonly IReporteDanoService _reporteService;

        public UserController(
            ISalaService salaService,
            IEquipoService equipoService,
            ISolicitudPrestamoService solicitudService,
            IReporteDanoService reporteService)
        {
            _salaService = salaService;
            _equipoService = equipoService;
            _solicitudService = solicitudService;
            _reporteService = reporteService;
        }

        // GET: User/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los datos
            var equipos = await _equipoService.GetEquipos();
            var solicitudes = await _solicitudService.GetSolicitudes();
            var reportes = await _reporteService.GetReportes();
            var salas = await _salaService.GetSalas();

            // Filtrar por usuario actual
            var equiposAsignados = equipos.Where(e => e.AsignadoAId == userId && e.Estado == EstadoEquipo.Asignado).ToList();
            var solicitudesUsuario = solicitudes.Where(s => s.UsuarioId == userId).ToList();
            var reportesUsuario = reportes.Where(r => r.UsuarioId == userId).ToList();

            // Calcular estadísticas
            ViewBag.EquiposAsignados = equiposAsignados.Count;
            ViewBag.EquiposAsignadosCount = equiposAsignados.Count;
            ViewBag.SolicitudesPendientes = solicitudesUsuario.Count(s => s.Estado == EstadoSolicitud.Pendiente);
            ViewBag.ReportesActivos = reportesUsuario.Count(r => r.Estado == EstadoReporte.Pendiente || r.Estado == EstadoReporte.EnRevision);
            ViewBag.SolicitudesAprobadas = solicitudesUsuario.Count(s => s.Estado == EstadoSolicitud.Aprobada);

            // Datos para las secciones
            ViewBag.EquiposAsignadosList = equiposAsignados;
            ViewBag.SolicitudesUsuario = solicitudesUsuario.OrderByDescending(s => s.FechaSolicitud).Take(5).ToList();
            ViewBag.Salas = salas.Where(s => s.Estado == EstadoSala.Activa).ToList();
            ViewBag.EquiposList = equipos;

            return View("User-Dashboard");
        }

        // POST: User/LiberarEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LiberarEquipo(Guid id)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                // Verificar que el equipo esté asignado al usuario actual
                var equipos = await _equipoService.GetEquipos();
                var equipo = equipos.FirstOrDefault(e => e.Id == id && e.AsignadoAId == userId);
                
                if (equipo == null)
                {
                    return Json(new { success = false, message = "No tienes permiso para liberar este equipo." });
                }

                await _equipoService.LiberarEquipo(id);
                return Json(new { success = true, message = "Equipo liberado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al liberar el equipo." });
            }
        }

        // GET: User/Salas
        public async Task<IActionResult> Salas()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var salas = await _salaService.GetSalas();
            var equipos = await _equipoService.GetEquipos();
            var solicitudes = await _solicitudService.GetSolicitudes();

            // Calcular disponibilidad para cada sala
            var salasConInfo = salas.Select(sala => new
            {
                Sala = sala,
                EquiposDisponibles = equipos.Count(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Disponible),
                EquiposAsignados = equipos.Count(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Asignado),
                EquiposEnMantenimiento = equipos.Count(e => e.SalaId == sala.Id && (e.Estado == EstadoEquipo.EnMantenimiento || e.Estado == EstadoEquipo.Dañado)),
                Estado = sala.Estado == EstadoSala.Activa && equipos.Any(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Disponible) 
                    ? "Libre" 
                    : sala.Estado == EstadoSala.Activa && !equipos.Any(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Disponible) && equipos.Any(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Asignado)
                    ? "Ocupada"
                    : sala.Estado == EstadoSala.Inactiva || equipos.Any(e => e.SalaId == sala.Id && (e.Estado == EstadoEquipo.EnMantenimiento || e.Estado == EstadoEquipo.Dañado)) && !equipos.Any(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Disponible)
                    ? "Mantenimiento"
                    : "Libre"
            }).ToList();

            // Calcular estadísticas para los badges
            var equiposAsignados = equipos.Where(e => e.AsignadoAId == userId && e.Estado == EstadoEquipo.Asignado).ToList();
            ViewBag.EquiposAsignadosCount = equiposAsignados.Count;
            ViewBag.SolicitudesPendientes = solicitudes.Count(s => s.UsuarioId == userId && s.Estado == EstadoSolicitud.Pendiente);

            ViewBag.SalasConInfo = salasConInfo;
            ViewBag.Equipos = equipos;

            return View();
        }

        // GET: User/GetSala/{id}
        [HttpGet]
        public async Task<IActionResult> GetSala(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null)
            {
                return Json(new { success = false, message = "Sala no encontrada." });
            }

            var equipos = await _equipoService.GetEquipos();
            var equiposDisponibles = equipos.Count(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Disponible);
            var equiposAsignados = equipos.Count(e => e.SalaId == sala.Id && e.Estado == EstadoEquipo.Asignado);
            var equiposEnMantenimiento = equipos.Count(e => e.SalaId == sala.Id && (e.Estado == EstadoEquipo.EnMantenimiento || e.Estado == EstadoEquipo.Dañado));

            return Json(new
            {
                success = true,
                sala = new
                {
                    id = sala.Id,
                    numero = sala.Numero,
                    ubicacion = sala.Ubicacion,
                    capacidad = sala.Capacidad,
                    estado = sala.Estado.ToString(),
                    totalEquipos = sala.TotalEquipos,
                    equiposDisponibles = equiposDisponibles,
                    equiposAsignados = equiposAsignados,
                    equiposEnMantenimiento = equiposEnMantenimiento,
                    usuarioNombre = sala.UsuarioNombre
                }
            });
        }

        // POST: User/SolicitarEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarEquipo(Guid SalaId, int TiempoEstimado)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                if (TiempoEstimado <= 0)
                {
                    return Json(new { success = false, message = "El tiempo estimado debe ser mayor a 0 horas." });
                }

                // Verificar que la sala exista y tenga equipos disponibles
                var sala = await _salaService.GetSala(SalaId);
                if (sala == null)
                {
                    return Json(new { success = false, message = "Sala no encontrada." });
                }

                if (sala.Estado != EstadoSala.Activa)
                {
                    return Json(new { success = false, message = "La sala no está disponible." });
                }

                var equipos = await _equipoService.GetEquipos();
                var equiposDisponibles = equipos.Count(e => e.SalaId == SalaId && e.Estado == EstadoEquipo.Disponible);
                if (equiposDisponibles == 0)
                {
                    return Json(new { success = false, message = "No hay equipos disponibles en esta sala." });
                }

                await _solicitudService.Create(new Services.Models.SolicitudModels.AddSolicitudModel
                {
                    UsuarioId = userId,
                    SalaId = SalaId,
                    EquipoId = null, // Sin equipo específico, se asigna cuando se aprueba
                    TiempoEstimado = TiempoEstimado
                });

                return Json(new { success = true, message = "Solicitud creada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear la solicitud: " + ex.Message });
            }
        }

        // GET: User/Equipos
        public async Task<IActionResult> Equipos()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los datos
            var equipos = await _equipoService.GetEquipos();
            var solicitudes = await _solicitudService.GetSolicitudes();
            var salas = await _salaService.GetSalas();

            // Filtrar equipos asignados al usuario actual
            var equiposAsignados = equipos.Where(e => e.AsignadoAId == userId && e.Estado == EstadoEquipo.Asignado).ToList();
            
            // Obtener solicitudes aprobadas del usuario para calcular tiempo de uso y historial
            var solicitudesAprobadas = solicitudes
                .Where(s => s.UsuarioId == userId && s.Estado == EstadoSolicitud.Aprobada && s.EquipoId.HasValue)
                .OrderByDescending(s => s.FechaAprobacion)
                .ToList();

            // Crear lista de equipos asignados con información adicional
            var equiposConInfo = equiposAsignados.Select(equipo =>
            {
                var solicitudEquipo = solicitudesAprobadas.FirstOrDefault(s => s.EquipoId == equipo.Id && s.Estado == EstadoSolicitud.Aprobada);
                var sala = salas.FirstOrDefault(s => s.Id == equipo.SalaId);
                
                var fechaAsignacion = solicitudEquipo?.FechaAprobacion ?? DateTime.UtcNow;
                var tiempoUso = DateTime.UtcNow - fechaAsignacion;
                var fechaRetornoEstimada = solicitudEquipo != null && solicitudEquipo.FechaAprobacion.HasValue
                    ? solicitudEquipo.FechaAprobacion.Value.AddHours(solicitudEquipo.TiempoEstimado)
                    : (DateTime?)null;

                return new
                {
                    Equipo = equipo,
                    Solicitud = solicitudEquipo,
                    Sala = sala,
                    FechaAsignacion = fechaAsignacion,
                    TiempoUso = tiempoUso,
                    FechaRetornoEstimada = fechaRetornoEstimada
                };
            }).ToList();

            // Crear historial de uso (todas las solicitudes aprobadas, incluso las ya liberadas)
            var historialUso = solicitudesAprobadas.Select(s =>
            {
                var equipoHistorial = equipos.FirstOrDefault(e => e.Id == s.EquipoId);
                var salaHistorial = salas.FirstOrDefault(sa => sa.Id == s.SalaId);
                var estaEnUso = equipoHistorial != null && equipoHistorial.Estado == EstadoEquipo.Asignado && equipoHistorial.AsignadoAId == userId;
                
                var fechaInicio = s.FechaAprobacion ?? s.FechaSolicitud;
                var fechaFin = s.FechaAprobacion.HasValue 
                    ? s.FechaAprobacion.Value.AddHours(s.TiempoEstimado) 
                    : (DateTime?)null;
                
                var duracion = fechaFin.HasValue && fechaFin.Value <= DateTime.UtcNow
                    ? fechaFin.Value - fechaInicio
                    : (estaEnUso ? DateTime.UtcNow - fechaInicio : (TimeSpan?)null);

                return new
                {
                    Solicitud = s,
                    Equipo = equipoHistorial,
                    Sala = salaHistorial,
                    EstaEnUso = estaEnUso,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Duracion = duracion
                };
            }).ToList();

            ViewBag.EquiposAsignados = equiposConInfo;
            ViewBag.HistorialUso = historialUso;
            ViewBag.EquiposAsignadosCount = equiposAsignados.Count;
            ViewBag.SolicitudesPendientes = solicitudes.Count(s => s.UsuarioId == userId && s.Estado == EstadoSolicitud.Pendiente);

            return View();
        }

        // GET: User/Solicitudes
        public async Task<IActionResult> Solicitudes(string estado = null)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var solicitudes = await _solicitudService.GetSolicitudes();
            var equipos = await _equipoService.GetEquipos();
            var salas = await _salaService.GetSalas();

            // Filtrar solicitudes del usuario actual
            var solicitudesUsuario = solicitudes.Where(s => s.UsuarioId == userId).ToList();

            // Aplicar filtro por estado si se especifica
            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                var estadoEnum = estado switch
                {
                    "Pendientes" => EstadoSolicitud.Pendiente,
                    "Aprobadas" => EstadoSolicitud.Aprobada,
                    "Rechazadas" => EstadoSolicitud.Rechazada,
                    _ => (EstadoSolicitud?)null
                };

                if (estadoEnum.HasValue)
                {
                    solicitudesUsuario = solicitudesUsuario.Where(s => s.Estado == estadoEnum.Value).ToList();
                }
            }

            // Ordenar por fecha de solicitud descendente
            solicitudesUsuario = solicitudesUsuario.OrderByDescending(s => s.FechaSolicitud).ToList();

            // Calcular estadísticas para los badges
            var equiposAsignados = equipos.Where(e => e.AsignadoAId == userId && e.Estado == EstadoEquipo.Asignado).ToList();
            ViewBag.EquiposAsignadosCount = equiposAsignados.Count;
            ViewBag.SolicitudesPendientes = solicitudes.Count(s => s.UsuarioId == userId && s.Estado == EstadoSolicitud.Pendiente);

            ViewBag.SolicitudesUsuario = solicitudesUsuario;
            ViewBag.EstadoFiltro = estado ?? "Todos";

            return View();
        }

        // POST: User/CancelarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarSolicitud(Guid id)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                await _solicitudService.CancelarSolicitud(id, userId);
                return Json(new { success = true, message = "Solicitud cancelada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al cancelar la solicitud: " + ex.Message });
            }
        }

        // GET: User/Reportes
        public async Task<IActionResult> Reportes(string estado = null)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var reportes = await _reporteService.GetReportes();
            var equipos = await _equipoService.GetEquipos();
            var salas = await _salaService.GetSalas();
            var solicitudes = await _solicitudService.GetSolicitudes();

            // Filtrar reportes del usuario actual
            var reportesUsuario = reportes.Where(r => r.UsuarioId == userId).ToList();

            // Aplicar filtro por estado si se especifica
            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                var estadoEnum = estado switch
                {
                    "Pendientes" => EstadoReporte.Pendiente,
                    "EnRevision" => EstadoReporte.EnRevision,
                    "Resueltos" => EstadoReporte.Resuelto,
                    "Rechazados" => EstadoReporte.Rechazado,
                    _ => (EstadoReporte?)null
                };

                if (estadoEnum.HasValue)
                {
                    reportesUsuario = reportesUsuario.Where(r => r.Estado == estadoEnum.Value).ToList();
                }
            }

            // Ordenar por fecha de reporte descendente
            reportesUsuario = reportesUsuario.OrderByDescending(r => r.FechaReporte).ToList();

            // Calcular estadísticas
            var equiposAsignados = equipos.Where(e => e.AsignadoAId == userId && e.Estado == EstadoEquipo.Asignado).ToList();
            ViewBag.EquiposAsignadosCount = equiposAsignados.Count;
            ViewBag.SolicitudesPendientes = solicitudes.Count(s => s.UsuarioId == userId && s.Estado == EstadoSolicitud.Pendiente);
            ViewBag.TotalReportes = reportesUsuario.Count;
            ViewBag.ReportesPendientes = reportesUsuario.Count(r => r.Estado == EstadoReporte.Pendiente || r.Estado == EstadoReporte.EnRevision);
            ViewBag.ReportesResueltos = reportesUsuario.Count(r => r.Estado == EstadoReporte.Resuelto);

            // Equipos disponibles para reportar (todos los equipos)
            var equiposDisponibles = equipos
                .Where(e => e.Estado != EstadoEquipo.Baja)
                .Select(e => new
                {
                    Id = e.Id,
                    Codigo = $"EQ-{e.Id.ToString().Substring(0, 8).ToUpper()}",
                    SalaNumero = salas.FirstOrDefault(s => s.Id == e.SalaId)?.Numero ?? "N/A"
                })
                .ToList();

            // Salas disponibles para reportar (solo salas activas)
            var salasDisponibles = salas
                .Where(s => s.Estado == EstadoSala.Activa)
                .Select(s => new
                {
                    Id = s.Id,
                    Numero = s.Numero,
                    Ubicacion = s.Ubicacion
                })
                .ToList();

            ViewBag.ReportesUsuario = reportesUsuario;
            ViewBag.EquiposDisponibles = equiposDisponibles;
            ViewBag.SalasDisponibles = salasDisponibles;
            ViewBag.EstadoFiltro = estado ?? "Todos";

            return View();
        }

        // POST: User/CrearReporte
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReporte(string TipoReporte, Guid? EquipoId, Guid? SalaId, string Descripcion)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                if (string.IsNullOrWhiteSpace(Descripcion))
                {
                    return Json(new { success = false, message = "La descripción del problema es obligatoria." });
                }

                if (string.IsNullOrEmpty(TipoReporte))
                {
                    return Json(new { success = false, message = "El tipo de reporte es obligatorio." });
                }

                var tipoEnum = TipoReporte switch
                {
                    "Equipo" => Domain.Enums.TipoReporte.Equipo,
                    "Sala" => Domain.Enums.TipoReporte.Sala,
                    _ => (Domain.Enums.TipoReporte?)null
                };

                if (!tipoEnum.HasValue)
                {
                    return Json(new { success = false, message = "Tipo de reporte inválido." });
                }

                if (tipoEnum.Value == Domain.Enums.TipoReporte.Equipo && !EquipoId.HasValue)
                {
                    return Json(new { success = false, message = "El código del equipo es obligatorio para reportes de equipo." });
                }

                if (tipoEnum.Value == Domain.Enums.TipoReporte.Sala && !SalaId.HasValue)
                {
                    return Json(new { success = false, message = "El nombre de la sala es obligatorio para reportes de infraestructura." });
                }

                var addModel = new Services.Models.ReporteModels.AddReporteModel
                {
                    UsuarioId = userId,
                    Tipo = tipoEnum.Value,
                    EquipoId = tipoEnum.Value == Domain.Enums.TipoReporte.Equipo ? EquipoId : null,
                    SalaId = tipoEnum.Value == Domain.Enums.TipoReporte.Sala ? SalaId : null,
                    Descripcion = Descripcion.Trim(),
                    Prioridad = Domain.Enums.PrioridadReporte.Media
                };

                await _reporteService.Create(addModel);
                return Json(new { success = true, message = "Reporte creado exitosamente. El reporte será enviado al coordinador de sala para su atención." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear el reporte: " + ex.Message });
            }
        }
    }
}

