using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = db.ApplicationUsers;
            ViewBag.UsersList = users;
            return View();
        }
        [Authorize(Roles = "User,Admin")]

        public IActionResult Profile()
        {
            return Redirect("/ApplicationUsers/Show/" + _userManager.GetUserId(User));

        }

        public async Task<ActionResult> Show(string id )
        {
            ApplicationUser user = db.ApplicationUsers.Include("Followers").Include("Following").Include("Pets").Where(u=>u.Id==id).First();
            ViewBag.UserCurent = user.Id;
            var roles = await _userManager.GetRolesAsync(user); 

            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.ApplicationUsers
                .Where(p => p.Id == id)
                .First();

            return View(user);           
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(string id, ApplicationUser requestUser)
        {
            ApplicationUser user = db.ApplicationUsers.Find(id);

            user.AllRoles = GetAllRoles();

            if (ModelState.IsValid)
            {
                user.UserName = requestUser.UserName;
                user.PhoneNumber = requestUser.PhoneNumber;
                user.ProfilePhoto = requestUser.ProfilePhoto;

                db.SaveChanges();
                return Redirect("/ApplicationUsers/Show/"+id);
            }
            else
            {
                return Redirect("/ApplicationUsers/Show/" + id);
            }
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.ApplicationUsers
                         .Include("Pets")
                         .Include("Comments")
                         .Include("BlogPosts")
                         .Where(u => u.Id == id)
                         .First();

            // Delete user pets
            if (user.Pets.Count > 0)
            {
                foreach (var pet in user.Pets)
                {
                    db.Pets.Remove(pet);
                }
            }
            // Delete user comments
            if (user.Comments.Count > 0)
            {
                foreach (var comment in user.Comments)
                {
                    db.Comments.Remove(comment);
                }
            }
            // Delete user blog posts
            if (user.BlogPosts.Count > 0)
            {
                foreach (var bp in user.BlogPosts)
                {
                    db.BlogPosts.Remove(bp);
                }
            }

            db.ApplicationUsers.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
