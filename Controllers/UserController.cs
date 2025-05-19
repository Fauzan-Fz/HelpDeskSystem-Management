using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;

            _signInManager = signInManager;

            _userManager = userManager;

            _context = context;
        }

        // GET: UserController
        public async Task<ActionResult> Index()
        {
            var users = await _context.Users
                .Include(x => x.Gender)
                .Include(x => x.Role)
                .ToListAsync();

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
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Gender"), "Id", "Description");
            ViewData["RoleId"] = new SelectList(_context.Roles.ToList(), "Id", "Name");

            return View();
        }

        // POST: UesrController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicationUser user)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser registeredUser = new();

                registeredUser.FirstName = user.FirstName;
                registeredUser.UserName = user.UserName;
                registeredUser.MiddleName = user.MiddleName;
                registeredUser.LastName = user.LastName;
                registeredUser.NormalizedEmail = user.UserName;
                registeredUser.Email = user.Email;
                registeredUser.EmailConfirmed = true;
                registeredUser.GenderId = user.GenderId;
                registeredUser.RoleId = user.RoleId;
                registeredUser.City = user.City;
                registeredUser.Country = user.Country;
                registeredUser.PhoneNumber = user.PhoneNumber;
                registeredUser.PasswordHash = user.PasswordHash;

                var result = await _userManager.CreateAsync(registeredUser, user.PasswordHash);

                if (result.Succeeded)
                {
                    TempData["Message"] = "User created successfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
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

        // POST: UserController/Delete/5
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