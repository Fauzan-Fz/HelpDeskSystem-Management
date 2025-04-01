using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                registeredUser.Gender = user.Gender;
                registeredUser.City = user.City;
                registeredUser.Country = user.Country;
                registeredUser.PhoneNumber = user.PhoneNumber;
                registeredUser.PasswordHash = user.PasswordHash;

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Password cannot be null or empty.");
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();

                var result = await _userManager.CreateAsync(registeredUser, user.PasswordHash);

                if (result.Succeeded)
                {
                    //Log the Audit Trail
                    var activity = new AuditTrail
                    {
                        Action = "Create",
                        TimeStamp = DateTime.Now,
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserId = userId,
                        Module = "Users",
                        AffectedTable = "Users"
                    };

                    TempData["Message"] = "User created successfully";

                    _context.Add(activity);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
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