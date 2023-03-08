using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;

namespace Petbook.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(ApplicationDbContext context)
        {
            db = context;
            _userManager = null;
            _roleManager = null;
        }
        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // display all posts with their comments and likes,
        // for each article display the pet who posted it
        // for each comment display the user who posted it
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var posts = db.Posts.Include("Pet")
                                .Include("PostLike")
                                .Include("Comments")
                                .Include("Comments.User")
                                .ToList();

            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // displays a post with the given id
        // with its comments, likes, the pet who posted it
        // and the user who posted each comment
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("Pet")
                                .Include("PostLike")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(p => p.PostId == id)
                                .First();

            return View(post);
        }

        // add a comment to one post from the db
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            else
            {
                Post p = db.Posts.Include("Pet")
                                .Include("PostLike")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(p => p.PostId == comment.PostId)
                                .First();

                return View(p);
            }
        }

       
        // form for adding a new post

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Post post = new Post();

            return View(post);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Post post)
        {

            post.PostDate = DateTime.Now;

            if (ModelState.IsValid)
            {

                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "The post was added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
        }

        // edit the post from the db
        // it will be displayed in a form with the current values
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Post post = db.Posts.Include("Pet")
                                .Where(p => p.PostId == id)
                                .First();


            // edit only the posts that are posted on the pets profile of the current user
            if (post.Pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Cannot edit the posts that aren't yours";
                return RedirectToAction("Index");
            }

        }

        // add the modified post in the db
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {

            Post post = db.Posts.Include("Pet")
                                .Where(p => p.PostId == id)
                                .First();

            if (ModelState.IsValid)
            {
                if (post.Pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {   
                    post.Photo = requestPost.Photo;
                    post.Description = requestPost.Description;
                    post.PostDate = requestPost.PostDate;
                    TempData["message"] = "The post has been modified";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot edit the posts that aren't yours";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestPost);
            }
        }

        // delete a post that is posted from the pets profiles of the current user
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Include("Pet")
                                .Where(p => p.PostId == id)
                                .First();
            if (post.Pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "The post has been deleted";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Cannot delete the posts that aren't yours";
                return RedirectToAction("Index");
            }
        }


    }
}
