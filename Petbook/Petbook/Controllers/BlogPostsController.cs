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
        public IActionResult Index()
        {
            if (User.IsInRole("User"))
            {
                var blogPosts = db.BlogPosts.Include("User")
                                            .Include("BlogPostTags")
                                            .Include("BlogPostLikes")
                              .Where(b => b.UserId == _userManager.GetUserId(User))
                              .ToList();
                ViewBag.BlogPosts = blogPosts;
                return View();
            }
            else
            {
                //the admin can see the blog posts of all users
                var blogPosts = db.BlogPosts.Include("User")
                                            .Include("BlogPostTags")
                                            .Include("BlogPostLikes")
                                            .ToList();
                ViewBag.BlogPosts = blogPosts;
                return View();
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
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
            return View(bp);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(BlogPost bp, [FromForm]string tag1)
        {
            bp.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.BlogPosts.Add(bp);
                db.SaveChanges();

                BlogPostTag bpt = new BlogPostTag();
                bpt.BlogPostId = bp.BlogPostId;
                bpt.TagId = 1;
                db.BlogPostTags.Add(bpt);
                db.SaveChanges();

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
