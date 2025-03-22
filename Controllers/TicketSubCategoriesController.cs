using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Controllers
{
    public class TicketSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketSubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketSubCategories
        public async Task<IActionResult> Index(int id)
        {
            var subCategories = await _context.TicketSubCategory
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .Where(x => x.CategoryId == id)
                .ToListAsync();

            return View(subCategories);
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
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Id");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TicketSubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Code,Name,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] TicketSubCategory ticketSubCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketSubCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Id", ticketSubCategory.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.ModifiedById);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Code,Name,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] TicketSubCategory ticketSubCategory)
        {
            if (id != ticketSubCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Id", ticketSubCategory.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", ticketSubCategory.ModifiedById);
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketSubCategoryExists(int id)
        {
            return _context.TicketSubCategory.Any(e => e.Id == id);
        }
    }
}
