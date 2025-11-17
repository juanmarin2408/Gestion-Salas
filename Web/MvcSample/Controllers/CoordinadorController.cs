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

        public CoordinadorController(
            ISalaService salaService, 
            IEquipoService equipoService,
            ISolicitudPrestamoService solicitudService,
            IReporteDanoService reporteService,
            IUserService userService)
        {
            _salaService = salaService;
            _equipoService = equipoService;
            _solicitudService = solicitudService;
            _reporteService = reporteService;
            _userService = userService;
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
            ViewBag.EquiposDisponibles = equipos.Count(e => e.Estado == EstadoEquipo.Disponible);
            ViewBag.Salas = salas;
            ViewBag.Equipos = equipos;

            return View("CoordinadorDashboard");
        }

        // GET: Coordinador/Solicitudes
        public async Task<IActionResult> Solicitudes()
        {
            var solicitudes = await _solicitudService.GetSolicitudes();
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            
            return View(solicitudes);
        }

        // POST: Coordinador/AprobarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSolicitud(Guid id)
        {
            // TODO: Implementar cuando tengas el servicio de solicitudes
            return Json(new { success = false, message = "Funcionalidad en desarrollo" });
        }

        // POST: Coordinador/RechazarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitud(Guid id, string MotivoRechazo)
        {
            // TODO: Implementar cuando tengas el servicio de solicitudes
            return Json(new { success = false, message = "Funcionalidad en desarrollo" });
        }

        // GET: Coordinador/Equipos
        public async Task<IActionResult> Equipos()
        {
            var equipos = await _equipoService.GetEquipos();
            // Obtener solo usuarios que tienen solicitudes (pendientes o aprobadas)
            var usuarios = await _solicitudService.GetUsuariosConSolicitudes();
            
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
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

                await _reporteService.ResolverReporte(id, coordinadorId, Observaciones);
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

        // GET: Coordinador/Calendario
        public async Task<IActionResult> Calendario(string fechaSemana, Guid? salaId)
        {
            // Parsear la fecha de la semana (lunes)
            DateTime fechaSemanaLunes;
            if (string.IsNullOrEmpty(fechaSemana))
            {
                // Si no hay fecha, usar la semana actual
                var hoy = DateTime.UtcNow.Date;
                var diaSemana = (int)hoy.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7; // Domingo = 7
                fechaSemanaLunes = hoy.AddDays(-(diaSemana - 1));
            }
            else
            {
                // Intentar parsear la fecha (puede venir en formato yyyy-MM-dd)
                if (!DateTime.TryParse(fechaSemana, out fechaSemanaLunes))
                {
                    // Si falla, intentar parsear solo la fecha sin hora
                    if (DateTime.TryParseExact(fechaSemana, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fechaSemanaLunes))
                    {
                        fechaSemanaLunes = fechaSemanaLunes.Date;
                    }
                    else
                    {
                        fechaSemanaLunes = DateTime.UtcNow.Date;
                    }
                }
                else
                {
                    fechaSemanaLunes = fechaSemanaLunes.Date;
                }
                
                // Asegurar que sea UTC
                if (fechaSemanaLunes.Kind != DateTimeKind.Utc)
                {
                    fechaSemanaLunes = DateTime.SpecifyKind(fechaSemanaLunes, DateTimeKind.Utc);
                }
                
                // Asegurar que sea el lunes de esa semana
                var diaSemana = (int)fechaSemanaLunes.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;
                fechaSemanaLunes = fechaSemanaLunes.AddDays(-(diaSemana - 1)).Date;
            }

            // Obtener todas las salas para el filtro
            var salas = await _salaService.GetSalas();
            
            // Si no hay sala seleccionada, usar la primera sala disponible
            if (!salaId.HasValue && salas.Any())
            {
                salaId = salas.First().Id;
            }
            
            // Obtener ocupaciones para la semana
            var ocupaciones = await _solicitudService.GetOcupacionesPorSemana(fechaSemanaLunes, salaId);
            
            ViewBag.SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientesCount();
            ViewBag.ReportesDanos = await _reporteService.GetReportesPendientesCount();
            ViewBag.Salas = salas;
            ViewBag.FechaSemana = fechaSemanaLunes;
            ViewBag.SalaFiltro = salaId;

            return View(ocupaciones);
        }
    }
}

