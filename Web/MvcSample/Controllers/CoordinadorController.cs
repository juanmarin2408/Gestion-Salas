using Microsoft.AspNetCore.Mvc;
using Services;
using Domain.Enums;
using Services.Models.SolicitudModels;
using MvcSample.Filters;

namespace MvcSample.Controllers
{
    [AuthorizeRole(RolUsuario.Coordinador)]
    public class CoordinadorController : Controller
    {
        private readonly ISalaService _salaService;
        private readonly IEquipoService _equipoService;

        public CoordinadorController(ISalaService salaService, IEquipoService equipoService)
        {
            _salaService = salaService;
            _equipoService = equipoService;
        }

        // GET: Coordinador/Dashboard
        public async Task<IActionResult> Index()
        {
            var salas = await _salaService.GetSalas();
            var equipos = await _equipoService.GetEquipos();

            // Calcular estadísticas para el coordinador
            ViewBag.SolicitudesPendientes = 8; // TODO: Implementar cuando tengas el modelo de solicitudes
            ViewBag.SolicitudesUrgentes = 3; // TODO: Implementar cuando tengas el modelo de solicitudes
            ViewBag.EquiposBloqueados = equipos.Count(e => e.Estado == EstadoEquipo.EnMantenimiento || e.Estado == EstadoEquipo.Dañado);
            ViewBag.ReportesDanos = 5; // TODO: Implementar cuando tengas el modelo de reportes
            ViewBag.EquiposDisponibles = equipos.Count(e => e.Estado == EstadoEquipo.Disponible);
            ViewBag.Salas = salas;
            ViewBag.Equipos = equipos;

            return View("CoordinadorDashboard");
        }

        // GET: Coordinador/Solicitudes
        public async Task<IActionResult> Solicitudes()
        {
            // TODO: Implementar cuando tengas el servicio de solicitudes
            ViewBag.SolicitudesPendientes = 8;
            ViewBag.ReportesDanos = 5;
            
            // Por ahora retornamos una lista vacía hasta que tengas el servicio
            return View(new List<Services.Models.SolicitudModels.SolicitudModel>());
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
    }
}

