using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
