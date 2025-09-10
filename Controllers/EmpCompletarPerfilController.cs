using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class EmpCompletarPerfilController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Esta es la "acción" que mostrará la vista. 
        [HttpGet] // Especificamos que responde a peticiones GET (cuando escribes la URL en el nav) ES TEMPORAL PARA PRUEBAS
        public IActionResult Completar()
        {
            // Esta línea busca y devuelve tu archivo .cshtml
            return View("EmpCompletarPerfil");
        }
    }
}
