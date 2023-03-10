using Microsoft.AspNetCore.Mvc;

namespace Petbook.Controllers
{
    public class BlogPostLikesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
