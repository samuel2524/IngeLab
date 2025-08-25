using IngeLab.Models;
using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class LoginController : Controller
    {
        public List<Usuarios> listaUsuarios = new List<Usuarios>()
        {
            new Usuarios(){ Nombre="admin", Contraseña="123"},
            new Usuarios(){ Nombre="user", Contraseña="12345"}
        };

        // CAMBIO 1: Renombrar Login a Index
        [HttpGet]
        public IActionResult Index() // <--- ANTES SE LLAMABA Login
        {
            return View();
        }

        // CAMBIO 2: Renombrar el método POST también a Index
        [HttpPost]
        public IActionResult Index(string nombre, string contraseña) // <--- ANTES SE LLAMABA Login
        {
            var usuarioValido = listaUsuarios
                .FirstOrDefault(u => u.Nombre == nombre && u.Contraseña == contraseña);

            if (usuarioValido != null)
            {
                return RedirectToAction("Principal", "Principal");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }
        }
    }
}