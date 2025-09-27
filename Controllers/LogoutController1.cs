using Microsoft.AspNetCore.Mvc;

namespace IngeLab.Controllers
{
    // ✨ AÑADE ESTA LÍNEA 👇
    [Route("Logout")]
    public class LogoutController : Controller
    {
        // ✨ Y AÑADE ESTA OTRA LÍNEA 👇
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
