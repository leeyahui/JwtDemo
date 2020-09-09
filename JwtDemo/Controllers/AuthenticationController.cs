using Microsoft.AspNetCore.Mvc;

namespace JwtDemo.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}