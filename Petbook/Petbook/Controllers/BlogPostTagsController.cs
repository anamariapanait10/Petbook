using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Petbook.Data;
using Petbook.Models;

namespace Petbook.Controllers
{
    public class BlogPostTagsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BlogPostTagsController(
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
        public IActionResult New(BlogPostTag requestBlogPostTag)
        {
            if (db.BlogPostTags
                .Where(bpt => bpt.BlogPostTagId == requestBlogPostTag.BlogPostTagId)
                .Where(bpt => bpt.BlogPostId == requestBlogPostTag.BlogPostId)
                .Where(bpt => bpt.TagId == requestBlogPostTag.TagId)
                .Count() > 0)
            {
                TempData["message"] = "This tag already exists for the blog post.";
            }
            else
            {
                db.BlogPostTags.Add(requestBlogPostTag);
                db.SaveChanges();
                TempData["message"] = "The tag was assigned to the blog post.";
            }
            var tags = db.Tags.ToList();
            ViewBag.Tags = tags;
            return Redirect("/BlogPosts/Show/" + requestBlogPostTag.BlogPostId);
        }


        [HttpPost]
        public IActionResult Delete(BlogPostTag requestBlogPostTag)
        {
            if (db.BlogPostTags
                .Where(bpt => bpt.BlogPostTagId == requestBlogPostTag.BlogPostTagId)
                .Where(bpt => bpt.BlogPostId == requestBlogPostTag.BlogPostId)
                .Where(bpt => bpt.TagId == requestBlogPostTag.TagId)
                .Count() > 0)
            {
                db.Remove(requestBlogPostTag);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "The blog post tag doesn't exist.";
            }

            return Redirect("/BlogPosts/Show/" + requestBlogPostTag.BlogPostId);
        }
    }
}
