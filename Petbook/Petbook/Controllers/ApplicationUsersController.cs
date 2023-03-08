using Microsoft.AspNetCore.Mvc;

namespace Petbook.Controllers
{
    public class ApplicationUsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
