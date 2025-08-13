using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    public class PrincipalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

