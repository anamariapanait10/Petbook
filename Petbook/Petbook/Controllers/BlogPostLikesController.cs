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

        [HttpPost("BlogPostLikes/IsLikedByCurrentUser/{blogPostId}")]
        public ActionResult<string> IsLikedByCurrentUser(int blogPostId)
        {
            var blogPostlike = db.BlogPostLikes
                           .Where(p => p.BlogPostId == blogPostId && p.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();
            if (blogPostlike == null)
            {
                return Ok("Yes");
            }
            else
            {
                return Ok("No");
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index(int blogPostId)
        {
            var blogpostlikes = db.BlogPostLikes
                                .Include("User")
                                .Include("BlogPost")
                                .Where(bpl => bpl.BlogPostId == blogPostId)
                                .ToList();
            ViewBag.BlogPostLikes = blogpostlikes;

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("BlogPostLikes/New/{id}")]
        public IActionResult New(int id)
        {
            var blogPostLike = db.BlogPostLikes
                           .Where(bp => bp.BlogPostId == id && bp.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();

            if (blogPostLike == null)
            {
                var like = new BlogPostLike();
                like.UserId = _userManager.GetUserId(User);
                like.BlogPostId = id;
                db.BlogPostLikes.Add(like);
                db.SaveChanges();
                return Ok("Blog post with id " + id + " liked");
            }
            else
            {
                DeleteLike(id);
                return Ok("Blog post with id " + id + " unliked");
            }
            
            //return RedirectToAction("BlogPosts", "Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        private void DeleteLike(int blogPostId)
        {
            var blogPostLike = db.BlogPostLikes
                           .Where(p => p.BlogPostId == blogPostId && p.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();
            if (blogPostLike != null)
            {
                db.Remove(blogPostLike);
                db.SaveChanges();
            }
        }
    }
}
