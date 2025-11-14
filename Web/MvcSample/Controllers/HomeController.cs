using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcSample.Models;
using Services;
using Services.Models.UserModels;
using System.Diagnostics;
using Services;
using Services.Models.UserModels;


namespace MvcSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;   // 👈 nuevo

        // Inyectamos también IUserService
        public HomeController(ILogger<HomeController> logger,
                              IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.HideFooter = true;   // para que no salga el footer en el login
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

        /* ------- LOGIN ------- */

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            // DEMO: credenciales "hardcodeadas"
            if (Email == "admin@universidad.edu" && Password == "Admin123!")
            {
                // Admin -> dashboard admin
                return RedirectToAction("Dashboard", "Admin");
            }

            if (Email == "coordinador@universidad.edu" && Password == "Coord123!")
            {
                // Coordinador -> (cuando tengas su controlador/vista)
                return RedirectToAction("Index", "Coordinator");
            }

            if (Email == "usuario@universidad.edu" && Password == "User123!")
            {
                // Usuario normal -> otra página
                return RedirectToAction("Index", "User");
            }

            // Si no coincide nada, volvemos al login con error
            ViewBag.HideFooter = true;
            ViewBag.LoginError = "Credenciales inválidas. Verifica tu correo y contraseña.";
            return View("Index");
        }

        /* ------- RECUPERAR CONTRASEÑA ------- */

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.HideFooter = true;
            return View();
        }


        /* ------- REGISTRO ------- */

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.HideFooter = true;
            return View(new AddUserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(AddUserModel model)
        {
            ViewBag.HideFooter = true;

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, volvemos a la vista
                return View(model);
            }

            try
            {
                await _userService.Register(model);   // guarda en la BD usando tu repositorio
                                                      // Después de registrar, lo mandamos al login
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                // Por ejemplo: “correo ya registrado”
                ViewBag.RegisterError = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                // MOSTRAR el error real mientras debugueas
                ViewBag.RegisterError = ex.Message;
                return View(model);
            }
        }


    }
}

