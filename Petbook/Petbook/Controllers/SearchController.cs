using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SearchController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager
           )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Results", new { search = search });
            }

            return View();
        }

        public IActionResult Results(string search)
        {
            if (string.IsNullOrEmpty(search)) { return RedirectToAction("Index"); }

            ViewBag.search = search;
            search = search.ToLower();
            var results = 0;

            var users = db.Users.Where(u => u.UserName.ToLower().Contains(search)
                                        || u.Email.ToLower().Contains(search)
                                      )
                        .Distinct()
                        .ToList();

            results += ViewBag.NbUsers = users.Count;

            ViewBag.Users = users;

            List<string> userNames = users != null ? users.Select(u => u.UserName).ToList() : new List<string>();

            var pets = db.Pets
                            .Include("Posts")
                            .Include("User")
                            .Where(p => (p.PetName != null && p.PetName.ToLower().Contains(search))
                                    || (p.Category != null && p.Category.ToLower().Contains(search))
                                    || userNames.Contains(p.User.UserName)
                            )
                            .Distinct()
                            .ToList();

            results += ViewBag.NbPets = pets.Count;

            ViewBag.Pets = pets;

            ViewBag.results = results;

            return View();
        }
    }
}
