﻿using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace HelpDeskSystem.Controllers
{
    [Authorize]
    public class TicketCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toasty;

        public TicketCategoriesController(ApplicationDbContext context, IToastNotification toast)
        {
            _context = context;
            _toasty = toast;
        }

        // GET: TicketCategories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketCategories.Include(t => t.CreatedBy).Include(t => t.ModifiedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // GET: TicketCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TicketCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCategory ticketCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ticketCategory.CreatedOn = DateTime.Now;
            ticketCategory.CreatedById = userId;

            _context.Add(ticketCategory);
            await _context.SaveChangesAsync(userId);

            _toasty.AddSuccessToastMessage("Ticket category created successfully",
                new ToastrOptions { Title = "Congratulation" });

            return RedirectToAction(nameof(Index));

            return View(ticketCategory);
        }

        // GET: TicketCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories.FindAsync(id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // POST: TicketCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketCategory ticketCategory)
        {
            if (id != ticketCategory.Id)
            {
                return NotFound();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ticketCategory.ModifiedOn = DateTime.Now;
                ticketCategory.ModifiedById = userId;

                _context.Update(ticketCategory);
                await _context.SaveChangesAsync(userId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketCategoryExists(ticketCategory.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _toasty.AddSuccessToastMessage("Ticket category updated successfully",
                new ToastrOptions { Title = "Congratulation" });

            return RedirectToAction(nameof(Index));

            return View(ticketCategory);
        }

        // GET: TicketCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // POST: TicketCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketCategory = await _context.TicketCategories.FindAsync(id);
            if (ticketCategory != null)
            {
                _context.TicketCategories.Remove(ticketCategory);
            }

            _toasty.AddSuccessToastMessage("Ticket category deleted successfully",
                new ToastrOptions { Title = "Congratulation" });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketCategoryExists(int id)
        {
            return _context.TicketCategories.Any(e => e.Id == id);
        }
    }
}