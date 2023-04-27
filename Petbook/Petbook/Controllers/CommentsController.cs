using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;
using System.IO;

namespace Petbook.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("Comments/New/")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New([FromBody] Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Ok(comment.CommentId + "&" + comment.UserId);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Comments/checkIsCurrentUser/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public ActionResult<string> checkIsCurrentUser([FromRoute] string userId)
        {
            if(userId == _userManager.GetUserId(User))
            {
                return Ok("Yes");
            } else
            {
                return Ok("No");
            }
        }

        [HttpPost("Comments/Delete/{commId}")]
        [Authorize(Roles = "User,Admin")]
        public ActionResult<string> Delete([FromRoute] int commId)
        {   
            Comment comment = db.Comments
                              .Include("Post")
                              .Include("Post.Pet.User")
                              .Where(c => c.CommentId == commId)
                              .First();

            db.Comments.Remove(comment);
            db.SaveChanges();
            return Ok("Comment deleted");
        }

        [HttpGet("Comments/GetCommentContent/{commId}")]
        [Authorize(Roles = "User,Admin")]
        public ActionResult<string> GetCommentContent(int commId)
        {
            Comment comment = db.Comments.Find(commId);
            return Ok(comment.CommentContent);
        }

        [HttpPost("Comments/Edit/{commId}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit([FromRoute] int commId, [FromForm] string commContent)
        {
            Comment comment = db.Comments.Find(commId);
            comment.CommentContent = commContent;

            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return Ok("Edited");      
            }
            else
            {
                return Ok("Error at editing");
            }
        }
    }
}
