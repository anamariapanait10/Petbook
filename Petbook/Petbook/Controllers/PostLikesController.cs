using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize(Roles = "User,Admin")]
        public IActionResult ShowNotifications()
        {
            ViewBag.CurrentUser = _userManager.GetUserId(User);

            var postIds = db.Posts.Include("Pet")
                                .Where(p => p.Pet.UserId == _userManager.GetUserId(User))
                                .OrderByDescending(p => p.PostDate)
                                .Select(p => p.PostId)
                                .ToList();

            if (postIds.Count > 0)
            {
                //comments
                var postNotifications = db.Comments
                                 .Include("User")
                                 .Include("Post")
                                 .Include("Post.Pet")
                                 .Where(p => postIds.Contains((int)p.PostId))
                                 .Where(p => p.UserId != _userManager.GetUserId(User))
                                 .Select(p => new Comment
                                 {
                                     CommentId = p.CommentId,
                                     PostId = p.PostId,
                                     CommentContent = "commented on ",
                                     CommentDate = p.CommentDate,
                                     User = p.User,
                                     Post = p.Post
                                 })
                                 .ToList();

                //likes
                postNotifications.AddRange(
                                db.PostLikes
                                 .Include("User")
                                 .Include("Post")
                                 .Include("Post.Pet")
                                 .Where(p => postIds.Contains((int)p.PostId))
                                 .Where(p => p.UserId != _userManager.GetUserId(User))
                                 .Select(p => new Comment
                                 {
                                     CommentId = 0,
                                     PostId = p.PostId,
                                     CommentContent = "liked ",
                                     CommentDate = p.AddedDate.HasValue ? p.AddedDate.Value : p.Post.PostDate.Value,
                                     User = p.User,
                                     Post = p.Post
                                 })
                                 .ToList()
                                 );

                //order notifications by date desc
                ViewBag.PostNotifications = postNotifications.OrderByDescending(n => n.CommentDate).ToList();
            }
            else
            {
                ViewBag.ErrorMessage = "No notifications";
            }

            return View();
        }
    }
}
