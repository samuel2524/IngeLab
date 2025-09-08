using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class VistaIngenierosController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        
    }
}