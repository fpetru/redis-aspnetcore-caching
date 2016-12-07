using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebCache.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var bytes = Encoding.UTF8.GetBytes("World");
            HttpContext.Session.Set("Hello", bytes);

            return View();
        }

        public IActionResult About()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("Hello", out bytes);
            var content = Encoding.UTF8.GetString(bytes);

            ViewData["Message"] = content;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
