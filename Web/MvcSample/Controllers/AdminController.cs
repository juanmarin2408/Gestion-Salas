using Microsoft.AspNetCore.Mvc;
using MvcSample.Models;
using Services;
using System.Diagnostics;

namespace MvcSample.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISalaService _salaService;
        private readonly IEquipoService _equipoService;
        private readonly IUserService _userService;

        public AdminController(
            ISalaService salaService,
            IEquipoService equipoService,
            IUserService userService)
        {
            _salaService = salaService;
            _equipoService = equipoService;
            _userService = userService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var salas = await _salaService.GetSalas();
            var equipos = await _equipoService.GetEquipos();
            var usuarios = await _userService.GetUsers();

            ViewBag.TotalSalas = salas.Count;
            ViewBag.TotalEquipos = equipos.Count;
            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.SalasActivas = salas.Count(s => s.Estado == Domain.Enums.EstadoSala.Activa);
            ViewBag.Salas = salas;
            ViewBag.Equipos = equipos;

            return View();
        }
    }
}
