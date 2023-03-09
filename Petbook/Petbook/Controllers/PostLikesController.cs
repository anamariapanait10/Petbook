using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class PostLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PostLikesController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager
           )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // add a like for a post in the db
        [HttpPost]
        public IActionResult New(PostLike p)
        {
            if (ModelState.IsValid)
            {
                db.PostLikes.Add(p);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + p.PostId);
            }
            else
            {
                return Redirect("/Posts/Show/" + p.PostId);
            }

        }

        // delete a like for a post in the db
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int postId, string userId)
        {   
            PostLike p = db.PostLikes.Where(p => p.PostId == postId && p.UserId == userId).First();
          
            if (p.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.PostLikes.Remove(p);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + p.PostId);
            }

            else
            {
                TempData["message"] = "You cannot unlike this post";
                return RedirectToAction("Index", "Posts");
            }
        }
    }
}
