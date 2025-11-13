
using Microsoft.AspNetCore.Mvc;
using MvcSample.Models;
using System.Diagnostics;

namespace MvcSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {

            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /* Controladores */

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            // DEMO: credenciales "hardcodeadas"
            if (Email == "admin@universidad.edu" && Password == "Admin123!")
            {
                // Admin -> dashboard admin
                return RedirectToAction("Dashboard", "admin");
            }

            if (Email == "coordinador@universidad.edu" && Password == "Coord123!")
            {
                // Coordinador -> cuando tengas su controlador/vista
                return RedirectToAction("Index", "Coordinator");
            }

            if (Email == "usuario@universidad.edu" && Password == "User123!")
            {
                // Usuario normal -> otro dashboard o página
                return RedirectToAction("Index", "User");
            }

            // Si no coincide nada, volvemos al login con error
            ViewBag.HideFooter = true;
            ViewBag.LoginError = "Credenciales inválidas. Verifica tu correo y contraseña.";
            return View("Index");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

    }
}
