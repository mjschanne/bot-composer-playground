using Microsoft.AspNetCore.Mvc;

namespace CustomDialogs.Controllers
{
    public class NotifyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
