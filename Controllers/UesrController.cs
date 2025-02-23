using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Controllers
{
    public class UesrController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UesrController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;

            _signInManager = signInManager;

            _userManager = userManager;

            _context = context;
        }

        // GET: UesrController
        public async Task<ActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: UesrController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UesrController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UesrController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UesrController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UesrController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UesrController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UesrController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
