using Microsoft.AspNetCore.Mvc;
using MvcSample.Models;
using System.Diagnostics;

namespace MvcSample.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Más adelante aquí puedes pasar un ViewModel con los datos reales
            return View();
        }

    }
}