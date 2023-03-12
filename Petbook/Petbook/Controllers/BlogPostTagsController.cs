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

        //adding a new blog post tag means assigning a given tag to a given blog post
        [HttpPost]
        public IActionResult New(BlogPostTag requestBlogPostTag)
        {
            if (ModelState.IsValid)
            {
                //if the tag already exists, we cannot add another one identical to it
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
            }
            else
            {
                TempData["message"] = "Couldn't assign the tag to the blog post.";
            }

            return Redirect("/BlogPosts/Show/" + requestBlogPostTag.BlogPostTagId);
        }

        //when a tag is deleted, we should remove the linkage with the blog post
        [HttpPost]
        public IActionResult Delete(BlogPostTag requestBlogPostTag)
        {
            if (ModelState.IsValid)
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
            }
            else
            {
                TempData["message"] = "Couldn't delete the blog post tag.";
            }

            return Redirect("/BlogPosts/Index/");
        }
    }
}
