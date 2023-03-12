﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult New(int blogPostId)
        {
            var blogPostLike = new BlogPostLike();
            blogPostLike.UserId = _userManager.GetUserId(User);
            blogPostLike.BlogPostId = blogPostId;
            db.BlogPostLikes.Add(blogPostLike);
            db.SaveChanges();

            return RedirectToAction("BlogPosts", "Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int blogPostId)
        {
            var blogPostLikes = db.BlogPostLikes
                               .Where(bpl => bpl.BlogPostId == blogPostId &&
                               bpl.UserId == _userManager.GetUserId(User))
                               .ToList();
            if (blogPostLikes != null)
            {
                db.Remove(blogPostLikes);
                db.SaveChanges();
            }

            return RedirectToAction("BlogPosts", "Index");
        }
    }
}