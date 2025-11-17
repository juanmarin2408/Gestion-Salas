using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.EquipoModels;
using Domain.Enums;

namespace MvcSample.Controllers
{
    public class EquipoController : Controller
    {
        private readonly IEquipoService _equipoService;
        private readonly ISalaService _salaService;

        public EquipoController(IEquipoService equipoService, ISalaService salaService)
        {
            _equipoService = equipoService;
            _salaService = salaService;
        }

        // GET: Equipo
        public async Task<IActionResult> Index()
        {
            var equipos = await _equipoService.GetEquipos();
            var salas = await _salaService.GetSalas();
            ViewBag.Salas = salas;
            return View("~/Views/Admin/Equipos.cshtml", equipos);
        }

        // POST: Equipo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEquipoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _equipoService.Create(model);
                return Json(new { success = true, message = "Equipo creado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear el equipo." });
            }
        }

        // POST: Equipo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AddEquipoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _equipoService.Update(id, model);
                return Json(new { success = true, message = "Equipo actualizado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al actualizar el equipo." });
            }
        }

        // POST: Equipo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _equipoService.Delete(id);
                return Json(new { success = true, message = "Equipo eliminado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al eliminar el equipo." });
            }
        }

        // GET: Equipo/GetEquipo/5 (para cargar datos en modal de edición)
        [HttpGet]
        public async Task<IActionResult> GetEquipo(Guid id)
        {
            var equipo = await _equipoService.GetEquipo(id);
            if (equipo == null)
            {
                return Json(new { success = false, message = "Equipo no encontrado." });
            }

            return Json(new 
            { 
                success = true, 
                equipo = new 
                { 
                    id = equipo.Id,
                    salaId = equipo.SalaId,
                    estado = (int)equipo.Estado
                } 
            });
        }

    }
}

