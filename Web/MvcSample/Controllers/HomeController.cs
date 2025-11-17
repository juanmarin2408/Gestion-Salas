using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcSample.Models;
using Services;
using Services.Models.UserModels;
using System.Diagnostics;


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
        public async Task<IActionResult> Login(string Email, string Password)
        {
            ViewBag.HideFooter = true;

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.LoginError = "Por favor ingresa tu correo y contraseña.";
                return View("Index");
            }

            try
            {
                var usuario = await _userService.LoginAsync(Email, Password);
                
                if (usuario == null)
                {
                    ViewBag.LoginError = "Credenciales inválidas. Verifica tu correo y contraseña.";
                    return View("Index");
                }

                // Guardar información del usuario en sesión
                HttpContext.Session.SetString("UserId", usuario.Id.ToString());
                HttpContext.Session.SetString("Documento", usuario.Documento);
                HttpContext.Session.SetString("Email", usuario.Email);
                HttpContext.Session.SetString("Nombre", usuario.Nombre);
                HttpContext.Session.SetString("Apellido", usuario.Apellido ?? "");
                HttpContext.Session.SetString("Rol", usuario.Rol.ToString());

                // Redirigir según el rol
                return usuario.Rol switch
                {
                    Domain.Enums.RolUsuario.Administrador => RedirectToAction("Dashboard", "Admin"),
                    Domain.Enums.RolUsuario.Usuario => RedirectToAction("Index", "User"),
                    Domain.Enums.RolUsuario.Coordinador => RedirectToAction("Index", "Coordinador"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login");
                ViewBag.LoginError = "Ocurrió un error al iniciar sesión. Por favor intenta de nuevo.";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        /* ------- RECUPERAR CONTRASEÑA ------- */

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.HideFooter = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { exists = false, message = "Por favor ingresa un correo." });
            }

            try
            {
                var exists = await _userService.EmailExistsAsync(email);
                return Json(new { exists = exists, message = exists ? "Correo encontrado." : "El correo no está registrado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar email");
                return Json(new { exists = false, message = "Ocurrió un error al verificar el correo." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                return Json(new { success = false, message = "Todos los campos son requeridos." });
            }

            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = "Las contraseñas no coinciden." });
            }

            if (newPassword.Length < 6)
            {
                return Json(new { success = false, message = "La contraseña debe tener al menos 6 caracteres." });
            }

            try
            {
                await _userService.ChangePasswordAsync(email, newPassword);
                return Json(new { success = true, message = "Contraseña actualizada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return Json(new { success = false, message = "Ocurrió un error al cambiar la contraseña." });
            }
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

