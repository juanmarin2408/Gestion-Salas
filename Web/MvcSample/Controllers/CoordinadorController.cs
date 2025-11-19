using Microsoft.AspNetCore.Mvc;
using Services;
using Domain.Enums;
using Services.Models.SolicitudModels;
using Services.Models.EquipoModels;
using Services.Models.ReporteModels;
using MvcSample.Filters;
using System;

namespace MvcSample.Controllers
{
    [AuthorizeRole(RolUsuario.Coordinador)]
    public class CoordinadorController : Controller
    {
        private readonly ISalaService _salaService;
        private readonly IEquipoService _equipoService;
        private readonly ISolicitudPrestamoService _solicitudService;
        private readonly IReporteDanoService _reporteService;
        private readonly IUserService _userService;
        private readonly IAsesoriaService _asesoriaService;

        public CoordinadorController(
            ISalaService salaService, 
            IEquipoService equipoService,
            ISolicitudPrestamoService solicitudService,
            IReporteDanoService reporteService,
            IUserService userService,
            IAsesoriaService asesoriaService)
        {
            _salaService = salaService;
            _equipoService = equipoService;
            _solicitudService = solicitudService;
            _reporteService = reporteService;
            _userService = userService;
            _asesoriaService = asesoriaService;
        }

        // GET: Coordinador/Dashboard
        public async Task<IActionResult> Index()
        {
            var salas = await _salaService.GetSalas();
            var equipos = await _equipoService.GetEquipos();

            // Calcular estadísticas para el coordinador desde la BD
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.SolicitudesUrgentes = await _solicitudService.GetSolicitudesUrgentesCount();
            ViewBag.EquiposBloqueados = equipos.Count(e => e.Estado == EstadoEquipo.EnMantenimiento || e.Estado == EstadoEquipo.Dañado);
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            ViewBag.AsesoriasPendientes = await _asesoriaService.GetAsesoriasPendientesCount();
            ViewBag.EquiposDisponibles = equipos.Count(e => e.Estado == EstadoEquipo.Disponible);
            ViewBag.Salas = salas;
            ViewBag.Equipos = equipos;

            // Obtener solicitudes pendientes para mostrar en el dashboard
            var solicitudesPendientes = await _solicitudService.GetSolicitudesByEstado(Domain.Enums.EstadoSolicitud.Pendiente);
            ViewBag.SolicitudesPendientesList = solicitudesPendientes.OrderByDescending(s => s.FechaSolicitud).Take(5).ToList();

            return View("CoordinadorDashboard");
        }

        // GET: Coordinador/Solicitudes
        public async Task<IActionResult> Solicitudes()
        {
            var solicitudes = await _solicitudService.GetSolicitudes();
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            ViewBag.AsesoriasPendientes = await _asesoriaService.GetAsesoriasPendientesCount();
            
            return View(solicitudes);
        }

        // POST: Coordinador/AprobarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSolicitud(Guid id)
        {
            var coordinadorIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out var coordinadorId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                await _solicitudService.AprobarSolicitud(id, coordinadorId);
                return Json(new { success = true, message = "Solicitud aprobada exitosamente. El equipo ha sido asignado al usuario." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al aprobar la solicitud: " + ex.Message });
            }
        }

        // POST: Coordinador/RechazarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitud(Guid id, string MotivoRechazo)
        {
            var coordinadorIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out var coordinadorId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                if (string.IsNullOrWhiteSpace(MotivoRechazo))
                {
                    return Json(new { success = false, message = "El motivo del rechazo es obligatorio." });
                }

                await _solicitudService.RechazarSolicitud(id, coordinadorId, MotivoRechazo);
                return Json(new { success = true, message = "Solicitud rechazada exitosamente." });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al rechazar la solicitud: " + ex.Message });
            }
        }

        // GET: Coordinador/Equipos
        public async Task<IActionResult> Equipos()
        {
            var equipos = await _equipoService.GetEquipos();
            // Obtener solo usuarios que tienen solicitudes (pendientes o aprobadas)
            var usuarios = await _solicitudService.GetUsuariosConSolicitudes();
            
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            ViewBag.AsesoriasPendientes = await _asesoriaService.GetAsesoriasPendientesCount();
            ViewBag.Usuarios = usuarios;
            
            return View(equipos);
        }

        // POST: Coordinador/AsignarEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarEquipo(Guid id, Guid UsuarioId)
        {
            try
            {
                await _equipoService.AsignarEquipo(id, UsuarioId);
                return Json(new { success = true, message = "Equipo asignado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al asignar el equipo." });
            }
        }

        // POST: Coordinador/BloquearEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloquearEquipo(Guid id, string MotivoBloqueo, PrioridadReporte PrioridadBloqueo)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Json(new { success = false, message = "El ID del equipo es requerido." });
                }

                if (string.IsNullOrWhiteSpace(MotivoBloqueo))
                {
                    return Json(new { success = false, message = "El motivo del bloqueo es requerido." });
                }

                await _equipoService.BloquearEquipo(id, MotivoBloqueo, PrioridadBloqueo);
                return Json(new { success = true, message = "Equipo bloqueado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return Json(new { success = false, message = $"Ocurrió un error al bloquear el equipo: {ex.Message}" });
            }
        }

        // POST: Coordinador/LiberarEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LiberarEquipo(Guid id)
        {
            try
            {
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

        // GET: Coordinador/Reportes
        public async Task<IActionResult> Reportes()
        {
            var reportes = await _reporteService.GetReportes();
            
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            ViewBag.AsesoriasPendientes = await _asesoriaService.GetAsesoriasPendientesCount();
            
            return View(reportes);
        }

        // POST: Coordinador/AtenderReporte
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtenderReporte(Guid id, string Observaciones)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Json(new { success = false, message = "El ID del reporte es requerido." });
                }

                if (string.IsNullOrWhiteSpace(Observaciones))
                {
                    return Json(new { success = false, message = "La solución aplicada es requerida." });
                }

                // Obtener el ID del coordinador desde la sesión
                var coordinadorIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out Guid coordinadorId))
                {
                    return Json(new { success = false, message = "No se pudo identificar al coordinador." });
                }

                await _reporteService.ResolverReporte(id, coordinadorId, Observaciones.Trim());
                return Json(new { success = true, message = "Reporte atendido exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error al atender el reporte: {ex.Message}" });
            }
        }

        // GET: Coordinador/Asesorias
        public async Task<IActionResult> Asesorias(string estado = null)
        {
            var asesorias = await _asesoriaService.GetAsesorias();

            // Aplicar filtro por estado si se especifica
            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                var estadoEnum = estado switch
                {
                    "Pendientes" => EstadoAsesoria.Pendiente,
                    "EnProceso" => EstadoAsesoria.EnProceso,
                    "Resueltos" => EstadoAsesoria.Resuelto,
                    "Rechazados" => EstadoAsesoria.Rechazado,
                    _ => (EstadoAsesoria?)null
                };

                if (estadoEnum.HasValue)
                {
                    asesorias = asesorias.Where(a => a.Estado == estadoEnum.Value).ToList();
                }
            }

            // Estadísticas
            ViewBag.Pendientes = await _asesoriaService.GetAsesoriasPendientesCount();
            ViewBag.EnProceso = await _asesoriaService.GetAsesoriasEnProcesoCount();
            ViewBag.Resueltos = await _asesoriaService.GetAsesoriasResueltasCount();

            ViewBag.Asesorias = asesorias;
            ViewBag.EstadoFiltro = estado ?? "Todos";

            return View();
        }

        // POST: Coordinador/AceptarAsesoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceptarAsesoria(Guid id)
        {
            var coordinadorIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out var coordinadorId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                await _asesoriaService.AceptarAsesoria(id, coordinadorId);
                return Json(new { success = true, message = "Asesoría aceptada exitosamente. Ahora está en proceso." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al aceptar la asesoría: " + ex.Message });
            }
        }

        // POST: Coordinador/RechazarAsesoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarAsesoria(Guid id, string MotivoRechazo)
        {
            var coordinadorIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out var coordinadorId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                if (string.IsNullOrWhiteSpace(MotivoRechazo))
                {
                    return Json(new { success = false, message = "El motivo del rechazo es obligatorio." });
                }

                await _asesoriaService.RechazarAsesoria(id, coordinadorId, MotivoRechazo);
                return Json(new { success = true, message = "Asesoría rechazada exitosamente." });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al rechazar la asesoría: " + ex.Message });
            }
        }

        // POST: Coordinador/ResolverAsesoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolverAsesoria(Guid id, string Observaciones)
        {
            var coordinadorIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(coordinadorIdString) || !Guid.TryParse(coordinadorIdString, out var coordinadorId))
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            try
            {
                await _asesoriaService.ResolverAsesoria(id, coordinadorId, Observaciones);
                return Json(new { success = true, message = "Asesoría marcada como completada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al resolver la asesoría: " + ex.Message });
            }
        }
    }
}

