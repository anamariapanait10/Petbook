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
                var blogPosts = from blogPost in db.BlogPosts.Include("User")
                              .Where(b => b.UserId == _userManager.GetUserId(User))
                                select blogPost;

                ViewBag.BlogPosts = blogPosts;
                return View();
            }

            else
            {
                //adminul poate vedea toate postarile tuturor userilor
                if (User.IsInRole("Admin"))
                {
                    var blogPosts = from blogPost in db.BlogPosts.Include("User")
                                    select blogPost;
                    ViewBag.BlogPosts = blogPosts;

                    return View();
                }

                else
                {
                    TempData["message"] = "Iti trebuie un cont pentru a vedea postarile";
                    //aici ar trebui redirectionare catre pagina de logare/ autentificare care nu exista inca
                    //return RedirectToAction("Index", "BlogPosts");
                    return View();
                }

            }

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            if (User.IsInRole("User"))
            {
                var blogPosts = db.BlogPosts
                                  .Include("BlogPostsLike")
                                  .Include("BlogPostsTag")
                                  .Include("User")
                                  .Where(b => b.BlogPostId == id)
                                  .Where(b => b.UserId == _userManager.GetUserId(User))
                                  .FirstOrDefault();
                if (blogPosts == null)
                {
                    TempData["message"] = "Postarea nu exista sau nu aveti drepturi de acces";
                    return RedirectToAction("Index", "BlogPosts");
                }

                return View(blogPosts);

            }

            else
            if (User.IsInRole("Admin"))
            {
                var blogPosts = db.BlogPosts
                                  .Include("BlogPostsLike")
                                  .Include("BlogPostsTag")
                                  .Include("User")
                                  .Where(b => b.BlogPostId == id)
                                  .FirstOrDefault();



                if (blogPosts == null)
                {
                    TempData["message"] = "Postarea cautata nu poate fi gasita";
                    return RedirectToAction("Index", "BlogPosts");
                }


                return View(blogPosts);
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi";
                //si aici ar trebui facuta redirectionare catre logare/ autentifcare
                return RedirectToAction("Index", "BlogPosts");
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult New(BlogPost bp)
        {
            bp.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.BlogPosts.Add(bp);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugata";
                return RedirectToAction("Index");
            }

            else
            {
                return View(bp);
            }
        }


        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int id)
        {

            BlogPost blogPost = db.BlogPosts.Include("User")
                                            .Include("BlogPostsLike")
                                            .Include("BlogPostsTag")
                                        .Where(bp => bp.BlogPostId == id)
                                        .Where(b => b.UserId == _userManager.GetUserId(User))
                                        .FirstOrDefault();


            if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(blogPost);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                return RedirectToAction("Index");
            }

        }

        // Se adauga postarea modificata in baza de date
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id, BlogPost requestBlogPost)
        {

            BlogPost blogPost = db.BlogPosts.Find(id);


            if (ModelState.IsValid)
            {
                if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {

                    blogPost.BlogPostContent = requestBlogPost.BlogPostContent;

                    TempData["message"] = "Postarea a fost modificata";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestBlogPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            BlogPost blogPost = db.BlogPosts.Include("BlogPostsLikes")
                                         .Include("BlogPostsTags")
                                         .Where(art => art.BlogPostId == id)
                                         .Where(b => b.UserId == _userManager.GetUserId(User))
                                         .FirstOrDefault();

            if (blogPost.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.BlogPosts.Remove(blogPost);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost stearsa";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine";
                return RedirectToAction("Index");
            }
        }

    }
}
