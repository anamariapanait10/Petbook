using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class BlogPostLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BlogPostLikesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User")]
        public IActionResult Show(int blogPostId)
        {
            var blogpostlikes = db.BlogPostLikes
                                .Include("User")
                                .Include("BlogPost")
                                .Where(bpl => bpl.BlogPostId == blogPostId)
                                .ToList();
            ViewBag.BlogPostLikes = blogpostlikes;

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult Delete(int blogPostId)
        {
            var blogPostLikes = db.BlogPostLikes
                               .Where(bpl => bpl.BlogPostId == blogPostId &&
                               bpl.UserId == _userManager.GetUserId(User))
                               .ToList();
            if(blogPostLikes != null)
            {
                db.Remove(blogPostLikes);
            }

            return RedirectToAction("BlogPosts", "Index");
        }

        [Authorize(Roles = "User")]
        public IActionResult New(int blogPostId)
        {
            var blogPostLike = new BlogPostLike();
            blogPostLike.UserId = _userManager.GetUserId(User);
            blogPostLike.BlogPostId = blogPostId;
            db.BlogPostLikes.Add(blogPostLike);

            return RedirectToAction("BlogPosts", "Index");
        }
    }
}
