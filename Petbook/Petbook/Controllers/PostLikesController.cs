using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                db.PostLikes.Add(p);
                db.SaveChanges();
                return Ok("Post with id " + postId + " liked");
            } else
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
    }
}
