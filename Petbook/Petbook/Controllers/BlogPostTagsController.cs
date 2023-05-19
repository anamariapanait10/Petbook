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
        [Authorize(Roles = "User,Admin")]
        public IActionResult Save([FromForm] int blogPostId, [FromForm] string tags)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                //retrieving tags ids based on tags names
                var newTags = tags.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var newTagIds = db.Tags
                                    .Where(t => newTags.Contains(t.TagName))
                                    .Select(t => t.TagId)
                                    .ToList();
                //retrieving existing tags ids associated with the blog post
                var existingTagIds = db.BlogPostTags
                                    .Where(t => t.BlogPostId == blogPostId)
                                    .Select(t => t.TagId)
                                    .ToList();
                //adding only tags that are not already associated with the blog post
                foreach (var tagId in newTagIds)
                {
                    if (!existingTagIds.Contains(tagId))
                    {
                        BlogPostTag bpt = new BlogPostTag();
                        bpt.BlogPostId = blogPostId;
                        bpt.TagId = tagId;
                        db.BlogPostTags.Add(bpt);
                    }
                }
                db.SaveChanges();
            }

            //all available tags added in ViewBag
            var allTags = db.Tags.ToList();
            ViewBag.Tags = allTags;

            return Redirect("/BlogPosts/Edit/" + blogPostId);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost("BlogPostTags/Delete/{blogPostId}/{tagId}")]
        public ActionResult<string> Delete(int blogPostId, int tagId)
        {
            //identifying blog post tag
            var blogPostTag = db.BlogPostTags
                                .Where(bpt => bpt.BlogPostId == blogPostId)
                                .Where(bpt => bpt.TagId == tagId)
                                .FirstOrDefault();

            //removing blog post tag if possible
            if (blogPostTag != null)
            {
                db.Remove(blogPostTag);
                db.SaveChanges();

                return Ok("Blog post tag deleted");
            }
            else
            {
                TempData["message"] = "The blog post tag doesn't exist.";

                return Ok("Blog post tag does not exist");
            }
        }
    }
}
