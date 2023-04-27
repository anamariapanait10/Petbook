using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BlogPostsController(
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
        public IActionResult Index(string search)
        {
            var allTags = db.Tags.ToList();
            ViewBag.Tags = allTags;
            ViewBag.SearchString = search;
            ViewBag.ShowViewBtn = true;

            //changes made so that any user can see all blogposts
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            var blogPosts = db.BlogPosts.Include("User")
                                            .Include("BlogPostTags")
                                            .Include("BlogPostTags.Tag")
                                            .Include("BlogPostLikes")
                                            .ToList();

            //filter results based on search keyword
            if (!string.IsNullOrEmpty(search))
            {
                var tagsIds = db.Tags.Where(t => t.TagName.ToLower().Contains(search.ToLower()))
                            .Select(t => t.TagId).ToList();

                blogPosts = blogPosts.Where(b => b.BlogPostTags.Where(bt => tagsIds.Contains(bt.TagId.Value)).Count() > 0).ToList();
            }

            ViewBag.BlogPosts = blogPosts;
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            var tags = db.Tags.ToList();
            ViewBag.Tags = tags;
            ViewBag.ShowViewBtn = false;
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            if (User.IsInRole("User"))
            {
                var blogPosts = db.BlogPosts
                                  .Include("BlogPostLikes")
                                  .Include("BlogPostTags")
                                  .Include("BlogPostTags.Tag")
                                  .Include("User")
                                  .Where(b => b.BlogPostId == id)
                                  .FirstOrDefault();
                
                if (blogPosts == null)
                {
                    TempData["message"] = "The blog post doesn't exist";
                    return RedirectToAction("Index", "BlogPosts");
                }
                                  
                return View(blogPosts);
            }
            else
            {
                var blogPosts = db.BlogPosts
                                  .Include("BlogPostLikes")
                                  .Include("BlogPostTags")
                                  .Include("User")
                                  .Where(b => b.BlogPostId == id)
                                  .FirstOrDefault();

                if (blogPosts == null)
                {
                    TempData["message"] = "The blog post could not be found.";
                    return RedirectToAction("Index", "BlogPosts");
                }
                return View(blogPosts);
            }
            
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            var tags = db.Tags.ToList();
            ViewBag.Tags = tags;
            BlogPost bp = new BlogPost();
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            return View(bp);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(BlogPost bp, [FromForm] string tag1)
        {
            bp.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.BlogPosts.Add(bp);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(tag1))
                {
                    var tags = tag1.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var tagIds = db.Tags.Where(t => tags.Contains(t.TagName)).Select(t => t.TagId).ToList();
                    foreach (var tagId in tagIds)
                    {
                        BlogPostTag bpt = new BlogPostTag();
                        bpt.BlogPostId = bp.BlogPostId;
                        bpt.TagId = tagId;
                        db.BlogPostTags.Add(bpt);
                    }
                    db.SaveChanges();
                }

                TempData["message"] = "The blog post was added.";

                return RedirectToAction("Index");
            }

            else
            {
                return View(bp);
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            BlogPost? blogPost = db.BlogPosts.Include("User")
                                            .Include("BlogPostLikes")
                                            .Include("BlogPostTags")
                                            .Where(bp => bp.BlogPostId == id)
                                            .FirstOrDefault();
            if(blogPost != null)
            {
                if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(blogPost);
                }

                else
                {
                    TempData["message"] = "This blog post can only be edited by the owner.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "This blog post doesn't exist.";
                return RedirectToAction("Index");
            }
        }

        //Add the modified blog post to the database
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, BlogPost requestBlogPost)
        {

            BlogPost blogPost = db.BlogPosts.Find(id);
            ViewBag.CurrentUser = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {

                    blogPost.BlogPostContent = requestBlogPost.BlogPostContent;
                    TempData["message"] = "The blog post was modified.";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "The blog post can only be edited by the owner.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestBlogPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            BlogPost? blogPost = db.BlogPosts.Include("BlogPostLikes")
                                         .Include("BlogPostTags")
                                         .Where(bp => bp.BlogPostId == id)
                                         .FirstOrDefault();
        ViewBag.CurrentUser = _userManager.GetUserId(User);
            if (blogPost != null)
            {
                if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.BlogPosts.Remove(blogPost);
                    db.SaveChanges();
                    TempData["message"] = "The blog post was deleted";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "The blog post can only be deleted by the owner.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "The blog post does not exist.";
                return RedirectToAction("Index");
            }
        }
    }
}
