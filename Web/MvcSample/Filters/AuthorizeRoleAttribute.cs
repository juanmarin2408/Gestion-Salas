using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MvcSample.Filters
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly RolUsuario[] _allowedRoles;

        public AuthorizeRoleAttribute(params RolUsuario[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Verificar si hay sesión
            var rolString = context.HttpContext.Session.GetString("Rol");
            var userId = context.HttpContext.Session.GetString("UserId");

            // Si no hay sesión o no hay rol, redirigir al login
            if (string.IsNullOrEmpty(rolString) || string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            // Intentar parsear el rol
            if (!Enum.TryParse<RolUsuario>(rolString, out var rol))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            // Verificar si el rol está permitido
            if (!_allowedRoles.Contains(rol))
            {
                // Si no tiene el rol permitido, redirigir según su rol
                var redirectAction = rol switch
                {
                    RolUsuario.Administrador => new RedirectToActionResult("Dashboard", "Admin", null),
                    RolUsuario.Coordinador => new RedirectToActionResult("Index", "Coordinador", null),
                    RolUsuario.Usuario => new RedirectToActionResult("Index", "User", null),
                    _ => new RedirectToActionResult("Index", "Home", null)
                };

                context.Result = redirectAction;
            }
        }
    }
}

