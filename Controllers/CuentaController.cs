using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; 
using IngeLab.Models;

namespace IngeLab.Controllers
{
    public class CuentaController : Controller
    {

        public List<Usuarios> listaUsuarios = new List<Usuarios>()
        {
            new Usuarios(){ Nombre="admin", Contraseña="123"},
            new Usuarios(){ Nombre="user", Contraseña="12345"}
        };


        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombre, string contraseña)
        {
             // Buscar usuario que coincida
            var usuarioValido = listaUsuarios
                .FirstOrDefault(u => u.Nombre == nombre && u.Contraseña == contraseña);

            if (usuarioValido != null)
            {
                // Si es válido → Redirige a otra página
                return RedirectToAction("Principal", "Principal");
            }
            else
            {
                // Si no es válido → Mostrar mensaje de error
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }
        }

    }
}
