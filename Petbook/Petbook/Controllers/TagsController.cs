using Microsoft.AspNetCore.Mvc;
using Petbook.Data;
using Petbook.Models;

namespace Petbook.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext db;
        public TagsController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var tags = db.Tags;
            ViewBag.Tags = tags;
            return View();
        }

        public ActionResult Show(int id)
        {
            Tag tags = db.Tags.Find(id);
            return View(tags);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Tag t)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(t);
                db.SaveChanges();
                TempData["message"] = "The tag was added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(t);
            }
        }

        public ActionResult Edit(int id)
        {
            Tag t = db.Tags.Find(id);
            return View(t);
        }

        [HttpPost]
        public ActionResult Edit(int id, Tag requestTag)
        {
            Tag tag = db.Tags.Find(id);
            if (ModelState.IsValid)
            {
                tag.TagName = requestTag.TagName;
                db.SaveChanges();
                TempData["message"] = "The tag was modified";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestTag);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Tag tag = db.Tags.Find(id);
            db.Tags.Remove(tag);
            TempData["message"] = "The tag was deleted";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
