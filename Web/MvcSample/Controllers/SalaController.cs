using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.SalaModels;
using Domain.Enums;
using MvcSample.Filters;

namespace MvcSample.Controllers
{
    [AuthorizeRole(RolUsuario.Administrador)]
    public class SalaController : Controller
    {
        private readonly ISalaService _salaService;

        public SalaController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        // GET: Sala
        public async Task<IActionResult> Index()
        {
            var salas = await _salaService.GetSalas();
            return View("~/Views/Admin/Salas.cshtml", salas);
        }

        // GET: Sala/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null)
            {
                return NotFound();
            }
            return View(sala);
        }

        // POST: Sala/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddSalaModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _salaService.Create(model);
                return Json(new { success = true, message = "Sala creada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear la sala." });
            }
        }

        // POST: Sala/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AddSalaModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _salaService.Update(id, model);
                return Json(new { success = true, message = "Sala actualizada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al actualizar la sala." });
            }
        }

        // POST: Sala/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _salaService.Delete(id);
                return Json(new { success = true, message = "Sala eliminada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al eliminar la sala." });
            }
        }

        // GET: Sala/GetSala/5 (para cargar datos en modal de edición)
        [HttpGet]
        public async Task<IActionResult> GetSala(Guid id)
        {
            var sala = await _salaService.GetSala(id);
            if (sala == null)
            {
                return Json(new { success = false, message = "Sala no encontrada." });
            }

            return Json(new 
            { 
                success = true, 
                sala = new 
                { 
                    id = sala.Id,
                    numero = sala.Numero,
                    capacidad = sala.Capacidad,
                    ubicacion = sala.Ubicacion,
                    estado = sala.Estado,
                    usuarioId = sala.UsuarioId
                } 
            });
        }
    }
}

