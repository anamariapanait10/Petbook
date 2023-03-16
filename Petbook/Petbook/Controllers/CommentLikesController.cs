using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class CommentLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentLikesController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager
           )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index(int commentId)
        {
            var commentLikes = db.CommentLikes
                                .Include("User")
                                .Include("Comment")
                                .Where(cl => cl.CommentId == commentId)
                                .ToList();
            ViewBag.CommentLikes = commentLikes;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(int commentId)
        {
            var commentLike = new CommentLike();
            commentLike.UserId = _userManager.GetUserId(User);
            commentLike.CommentId = commentId;
            db.CommentLikes.Add(commentLike);
            db.SaveChanges();

            return RedirectToAction("Comments/Show/" + commentId);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int commentId)
        {
            var commentLike = db.CommentLikes
                               .Where(bpl => bpl.CommentId == commentId &&
                               bpl.UserId == _userManager.GetUserId(User))
                               .FirstOrDefault();
            if (commentLike != null)
            {
                db.Remove(commentLike);
                db.SaveChanges();
            }

            return RedirectToAction("Comments/Show/" + commentId);
        }
    }
}
