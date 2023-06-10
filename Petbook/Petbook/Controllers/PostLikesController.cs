using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;
using System.Diagnostics.Eventing.Reader;

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

        [Authorize(Roles = "User,Admin")]
        [HttpPost("PostLikes/IsLikedByCurrentUser/{postId}")]
        public ActionResult<string> IsLikedByCurrentUser(int postId)
        {
            var postlike = db.PostLikes
                           .Where(p => p.PostId == postId && p.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();
            if (postlike == null)
            {
                return Ok("Yes");
            }
            else
            {
                return Ok("No");
            }
        }

        // add a like for a post in the db
        [Authorize(Roles = "User,Admin")]
        [HttpPost("PostLikes/AddLike/{postId}")]
        public ActionResult<string> AddLike(int postId)
        {
            var postlike = db.PostLikes
                           .Where(p => p.PostId == postId && p.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();
            if (postlike == null)
            {
                var p = new PostLike();
                p.PostId = postId;
                p.UserId = _userManager.GetUserId(User);
                p.AddedDate = DateTime.Now;
                db.PostLikes.Add(p);
                db.SaveChanges();
                return Ok("Post with id " + postId + " liked");
            }
            else
            {
                DeleteLike(postId);
                return Ok("Post with id " + postId + " unliked");
            }
        }

        // unlike a post that was liked
        [Authorize(Roles = "User,Admin")]
        private void DeleteLike(int postId)
        {
            var postlike = db.PostLikes
                           .Where(p => p.PostId == postId && p.UserId == _userManager.GetUserId(User))
                           .FirstOrDefault();
            if (postlike != null)
            {
                db.PostLikes.Remove(postlike);
                db.SaveChanges();
            }
        }

        [Authorize(Roles = "User,Admin")]
        private IActionResult ShowNotifications()
        {
            ViewBag.CurrentUser = _userManager.GetUserId(User);

            var postIds = db.Posts.Include("Pet")
                                .Include("Pet.User")
                                .Include("PostLikes")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(p => p.Pet.UserId == _userManager.GetUserId(User))
                                .OrderByDescending(p => p.PostDate)
                                .Select(p => p.PostId)
                                .ToList();



            var postLikes = db.PostLikes
                                 .Where(p => postIds.Any(id => id == p.PostId))
                                 .OrderByDescending(p => p.AddedDate)
                                 .ToList();
            ViewBag.PostLikes = postLikes;

            return View();
        }
    }
}

