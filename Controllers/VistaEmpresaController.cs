using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class VistaEmpresaController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        
    }
}