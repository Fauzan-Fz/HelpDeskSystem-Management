using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace HelpDeskSystem.Controllers
{
    public class TicketSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toasty;


        public TicketSubCategoriesController(ApplicationDbContext context, IToastNotification toasty)
        {
            _context = context;
            _toasty = toasty;
        }

        // GET: TicketSubCategories
        public async Task<IActionResult> Index(int id, TicketSubCategoriesVM vm)
        {
            vm.TicketSubCategories = await _context.TicketSubCategory
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .Where(x => x.CategoryId == id)
                .ToListAsync();

            vm.CategoryId = id;

            return View(vm);
        }

        // GET: TicketSubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategory
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        // GET: TicketSubCategories/Create
        public IActionResult Create(int Id)
        {
            //ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Id");
            //ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id");
            TicketSubCategory category = new();
            category.CategoryId = Id;
            return View(category);
        }

        // POST: TicketSubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, TicketSubCategory ticketSubCategory)
        {
            var loginUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ticketSubCategory.CreatedById = loginUser;
            ticketSubCategory.CreatedOn = DateTime.Now;

            ticketSubCategory.Id = 0;
            ticketSubCategory.CategoryId = Id;
            _context.Add(ticketSubCategory);
            await _context.SaveChangesAsync();

            var activity = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserId = loginUser,
                Module = "Ticket Sub-Categories",
                AffectedTable = "TicketSubCategory"
            };

            _toasty.AddSuccessToastMessage("Ticket Sub Category created successfully",
                new ToastrOptions { Title = "Congratulation" });


            return RedirectToAction(nameof(Index));

            return View(ticketSubCategory);
        }

        // GET: TicketSubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategory.FindAsync(id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Id", ticketSubCategory.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.ModifiedById);
            return View(ticketSubCategory);
        }

        // POST: TicketSubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketSubCategory ticketSubCategory)
        {
            if (id != ticketSubCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var loginUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ticketSubCategory.ModifiedById = loginUser;
                    ticketSubCategory.ModifiedOn = DateTime.Now;

                    var activity = new AuditTrail
                    {
                        Action = "Update",
                        TimeStamp = DateTime.Now,
                        IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserId = loginUser,
                        Module = "Ticket Sub-Categories",
                        AffectedTable = "TicketSubCategory"
                    };

                    _toasty.AddSuccessToastMessage("Ticket Sub Category updated successfully",
                new ToastrOptions { Title = "Congratulation" });

                    _context.Update(ticketSubCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketSubCategoryExists(ticketSubCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticketSubCategory);
        }

        // GET: TicketSubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategory
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        // POST: TicketSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketSubCategory = await _context.TicketSubCategory.FindAsync(id);
            if (ticketSubCategory != null)
            {
                _context.TicketSubCategory.Remove(ticketSubCategory);
            }

            var loginUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activity = new AuditTrail
            {
                Action = "Delete",
                TimeStamp = DateTime.Now,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserId = loginUser,
                Module = "Ticket Sub-Categories",
                AffectedTable = "TicketSubCategory"
            };

            _toasty.AddSuccessToastMessage("Ticket Sub Category deleted successfully",
                new ToastrOptions { Title = "Congratulation" });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketSubCategoryExists(int id)
        {
            return _context.TicketSubCategory.Any(e => e.Id == id);
        }
    }
}
