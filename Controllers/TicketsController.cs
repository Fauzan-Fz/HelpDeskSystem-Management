using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace HelpDeskSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toasty;
        private readonly IConfiguration _configuration;

        public TicketsController(ApplicationDbContext context, IToastNotification toasty, IConfiguration configuration)
        {
            _context = context;
            _toasty = toasty;
            _configuration = configuration;
        }

        // GET: Tickets //
        public async Task<IActionResult> Index(TicketViewModel vm)
        {
            vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments) // Include untuk mengambil data Comments
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> TicketsComments(string Id)
        {
            var comment = await _context.Comments.Where(t => t.TicketId == Id)
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .ToListAsync();
            return View(comment);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(string id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel ticketvm, IFormFile attachment)
        {
            if (attachment.Length > 0)
            {
                var filename = "Ticket_Attachment" + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + attachment.FileName;
                var path = _configuration["FileSettings:UploadsFolder"];
                var filepath = Path.Combine(path, filename);
                var stream = new FileStream(filepath, FileMode.Create);
                await attachment.CopyToAsync(stream);
                ticketvm.Attachment = filename;
            }

            var pendingStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Status" && x.Code == "Pending")
                .FirstOrDefaultAsync();

            Ticket ticket = new();
            ticket.Id = ticketvm.Id;
            ticket.Title = ticketvm.Title;
            ticket.Description = ticketvm.Description;
            ticket.StatusId = pendingStatus.Id;
            ticket.PriorityId = ticketvm.PriorityId;
            ticket.SubCategoryId = ticketvm.SubCategoryId;
            ticket.Attachment = ticketvm.Attachment;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ticket.CreatedOn = DateTime.Now;
            ticket.CreatedById = userId;
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserId = userId,
                Module = "Ticket",
                AffectedTable = "Tickets"
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();

            _toasty.AddSuccessToastMessage("Ticket created successfully",
                new ToastrOptions { Title = "Congratulation" });

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string id, TicketViewModel vm) // Add Comment method
        {
            // Add the current user's ID to the comment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Comment newComment = new(); // Create a new Comment object

            newComment.TicketId = id; // Assign the ticket ID
            newComment.CreatedOn = DateTime.Now; // Take the current date and time as the comment creation date
            newComment.CreatedById = userId; // Assign the current user's ID as the comment creator
            newComment.Description = vm.CommentDescription; // Assign the comment description entered by the user

            _context.Add(newComment); // Add the new comment to the database
            await _context.SaveChangesAsync(); // Save the changes

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Create", // Set the action to "Create" for audit trail
                TimeStamp = DateTime.Now, // Set the current date and time
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), // Get the client's IP address
                UserId = userId, // Assign the current user's ID
                Module = "Comments", // Assign the module name
                AffectedTable = "Comments" // Assign the affected table
            };

            _context.Add(activity); // Add the audit trail entry
            await _context.SaveChangesAsync(); // Save the changes

            return RedirectToAction("Details", new { id = id }); // Redirect to the Details page after adding the comment description
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,Status,Priority,CreatedById,CreatedOn")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activity = new AuditTrail
                {
                    Action = "Update",
                    TimeStamp = DateTime.Now,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserId = userId,
                    Module = "Ticket",
                    AffectedTable = "Tickets"
                };

                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _toasty.AddSuccessToastMessage("Ticket updated successfully",
                 new ToastrOptions { Title = "Congratulation" });

            return RedirectToAction(nameof(Index));

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activity = new AuditTrail
            {
                Action = "Deleted",
                TimeStamp = DateTime.Now,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserId = userId,
                Module = "Ticket",
                AffectedTable = "Tickets"
            };

            await _context.SaveChangesAsync();

            _toasty.AddSuccessToastMessage("Ticket deleted successfully",
                new ToastrOptions { Title = "Congratulation" });

            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(string id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}