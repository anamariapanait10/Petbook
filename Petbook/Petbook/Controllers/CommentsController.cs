using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

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

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Comment comment)
        {

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }

            else
            {
                return Redirect("/Posts/Show/" + comment.PostId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int commentId)
        {
            Comment comment = db.Comments
                              .Include("Post")
                              .Include("Post.Pet.User")
                              .Where(c => c.CommentId == commentId)
                              .First();
            if (comment.Post.Pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.CommentId);
            }

            else
            {
                TempData["message"] = "You cannot delete a comment which isn't yours";
                return RedirectToAction("Index", "Posts");
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int commentId)
        {
            Comment comment = db.Comments.Find(commentId);

            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comment);
            }

            else
            {
                TempData["message"] = "You cannot edit a comment which isn't yours";
                return RedirectToAction("Index", "Posts");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int commentId, Comment requestComment)
        {
            Comment comment = db.Comments.Find(commentId);

            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comment.CommentContent = requestComment.CommentContent;
                    db.SaveChanges();

                    return Redirect("/Posts/Show/" + comment.PostId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "You cannot make changes on this comment.";
                return RedirectToAction("Index", "Posts");
            }
        }
    }
}
