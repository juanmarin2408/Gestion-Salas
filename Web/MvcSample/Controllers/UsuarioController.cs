using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.UserModels;
using Domain.Enums;
using MvcSample.Filters;

namespace MvcSample.Controllers
{
    [AuthorizeRole(RolUsuario.Administrador)]
    public class UsuarioController : Controller
    {
        private readonly IUserService _userService;

        public UsuarioController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userService.GetUsers();
            return View("~/Views/Admin/Usuarios.cshtml", usuarios);
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _userService.Register(model);
                return Json(new { success = true, message = "Usuario creado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear el usuario." });
            }
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AddUserModel model)
        {
            // En edición, la contraseña es opcional
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove(nameof(model.Password));
                ModelState.Remove(nameof(model.ConfirmPassword));
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Por favor completa todos los campos correctamente." });
            }

            try
            {
                await _userService.Update(id, model);
                return Json(new { success = true, message = "Usuario actualizado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al actualizar el usuario." });
            }
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userService.Delete(id);
                return Json(new { success = true, message = "Usuario eliminado exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ocurrió un error al eliminar el usuario." });
            }
        }

        // GET: Usuario/GetUsuario/5 (para cargar datos en modal de edición)
        [HttpGet]
        public async Task<IActionResult> GetUsuario(Guid id)
        {
            var usuario = await _userService.GetUser(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            return Json(new 
            { 
                success = true, 
                usuario = new 
                { 
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    documento = usuario.Documento,
                    email = usuario.Email,
                    rol = (int)usuario.Rol
                } 
            });
        }
    }
}

