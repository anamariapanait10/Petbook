using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petbook.Data;
using Petbook.Models;
using System.Data;

namespace Petbook.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PetsController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager
           )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // display all pets with their owner and posts
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var pets = db.Pets.Include("Posts")
                                .Include("User")
                                .ToList();

            ViewBag.Pets = pets;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // display a pet with his owner and posts
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            var pet = db.Pets.Include("Posts")
                                .Include("User")
                                .Where(p => p.PetId == id)
                                .First();
            ViewBag.UserCurent = pet.UserId;
            return View(pet);
        }


        // form for adding a new pet
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Pet pet = new Pet();

            return View(pet);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Pet pet)
        {

            if (ModelState.IsValid)
            {
                db.Pets.Add(pet);
                db.SaveChanges();
                TempData["message"] = "The pet was added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(pet);
            }
        }

        // edit the pet from the db
        // it will be displayed in a form with the current values
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Pet pet = db.Pets.Include("Posts")
                            .Include("User")
                            .Where(p => p.PetId == id)
                            .First();

            // edit only the pets that are owned by the current user
            if (pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(pet);
            }
            else
            {
                TempData["message"] = "Cannot edit the pets that aren't yours";
                return RedirectToAction("Index");
            }
        }

        // add the modified pet in the db
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Pet requestPet)
        {

            Pet pet = db.Pets.Include("Posts")
                            .Include("User")
                            .Where(p => p.PetId == id)
                            .First();

            if (ModelState.IsValid)
            {
                if (pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {   
                    pet.PetName = requestPet.PetName;
                    pet.Category= requestPet.Category;
                    pet.Description = requestPet.Description;
                    pet.Location = requestPet.Location;
                    TempData["message"] = "The pet has been modified";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot edit the pets that aren't yours";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestPet);
            }
        }

        // delete a pet that is owned by the current user
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Pet pet = db.Pets.Include("Posts")
                            .Include("User")
                            .Where(p => p.PetId == id)
                            .First();
            if (pet.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Pets.Remove(pet);
                db.SaveChanges();
                TempData["message"] = "The pet has been deleted";
                return Redirect("/ApplicationUsers/Show/" + pet.UserId);
            }
            else
            {
                TempData["message"] = "Cannot delete the pets that aren't yours";
                return Redirect("/ApplicationUsers/Show/" + pet.UserId);
            }
        }
    }
}
