using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class IngCompletarPerfilController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Esta es la "acción" que mostrará tu vista.
        [HttpGet] // Especificamos que responde a peticiones GET (cuando escribes la URL en el nav)
        public IActionResult Completar()
        {
            // Esta línea busca y devuelve tu archivo .cshtml
            return View("CompletarPerfil");
        }
    }
}
