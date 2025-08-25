using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class RegistroController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string Correo, string Contraseña) // Simplificado por ahora
        {
            // AQUÍ IRÍA LA LÓGICA PARA GUARDAR EL NUEVO USUARIO EN LA BASE DE DATOS
            // Por ahora, solo lo redirigimos al Login.

            // Redirige al login después del registro exitoso
            return RedirectToAction("Index", "Login");
        }
    }
}