using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class RegistroController : Controller
    {
        // Aquí puedes agregar métodos para manejar el registro de usuarios
        // Por ejemplo, un método para mostrar el formulario de registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // // Método para manejar el envío del formulario de registro
        // [HttpPost]
        // public IActionResult Register(string nombre, string contraseña)
        // {
        //     // Aquí puedes agregar la lógica para guardar el nuevo usuario
        //     // Por ejemplo, agregarlo a una lista o base de datos

        //     // Redirigir al login después del registro exitoso
        //     return RedirectToAction("Login", "Cuenta");
        // }
    }
}