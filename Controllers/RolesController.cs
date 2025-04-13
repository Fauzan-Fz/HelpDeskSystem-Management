using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Semua yang berhubungan dengan user
        private readonly RoleManager<IdentityRole> _roleManager; // Semua yang berhubungan dengan role untuk user
        private readonly SignInManager<ApplicationUser> _signInManager; // Semua yang berhubungan dengan login user
        private readonly ApplicationDbContext _context; // Semua yang berhubungan dengan database

        public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager) // Constructor untuk controller
        {
            _roleManager = roleManager;

            _signInManager = signInManager;

            _userManager = userManager;

            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync(); // Ambil semua role dari database
            return View(roles);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RolesViewModel vm)
        {
            IdentityRole role = new IdentityRole(); // Buat object role baru
            role.Name = vm.RoleName; // Set nama role dari view model

            var result = await _roleManager.CreateAsync(role); // Simpan role ke database
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }
        }
    }
}