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


            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments) // Include untuk mengambil data Comments
                .OrderBy(x => x.CreatedOn)
                .AsQueryable(); // Untuk mencarikan data dari database

            if (vm != null && !string.IsNullOrEmpty(vm.Title))
            {
                alltickets = alltickets
                    .Where(x => x.CreatedById == vm.CreatedById);
            }

            if (vm != null && !string.IsNullOrEmpty(vm.Title))
            {
                alltickets = alltickets
                    .Where(x => x.CreatedById == vm.CreatedById);
            }

            if (vm != null && vm.StatusId > 0)
            {
                alltickets = alltickets
                    .Where(x => x.StatusId == vm.StatusId);
            }

            if (vm != null && vm.PriorityId > 0)
            {
                alltickets = alltickets
                    .Where(x => x.PriorityId == vm.PriorityId);
            }
            if (vm != null && vm.CategoryId > 0)
            {
                alltickets = alltickets
                    .Where(x => x.SubCategory.CategoryId == vm.PriorityId);
            }

            vm.Tickets = await alltickets.ToListAsync();

            // ViewData
            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");


            return View(vm);
        }

        public async Task<IActionResult> AssignedTickets(TicketViewModel vm)
        {
            var assignStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Description == "Assigned")
                .FirstOrDefaultAsync();

            vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments) // Include untuk mengambil data Comments
                .OrderBy(x => x.CreatedOn)
                .Where(x => x.StatusId == assignStatus.Id)
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> ClosedTickets(TicketViewModel vm)
        {
            var closedStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Description == "Closed")
                .FirstOrDefaultAsync();

            vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments) // Include untuk mengambil data Comments
                .OrderBy(x => x.CreatedOn)
                .Where(x => x.StatusId == closedStatus.Id)
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> ResolvedTickets(TicketViewModel vm)
        {
            var resolveStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Description == "Resolved")
                .FirstOrDefaultAsync();

            vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments) // Include untuk mengambil data Comments
                .OrderBy(x => x.CreatedOn)
                .Where(x => x.StatusId == resolveStatus.Id)
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
                 .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolution = await _context.TicketResolutions
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Include(t => t.Status)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Tickets/Details/Resolve
        public async Task<IActionResult> Resolve(string id, TicketViewModel vm)
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
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolution = await _context.TicketResolutions
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Include(t => t.Status)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        // GET: Tickets/Details/Re-Open
        public async Task<IActionResult> ReOpen(string id, TicketViewModel vm)
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
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolution = await _context.TicketResolutions
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Include(t => t.Status)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        public async Task<IActionResult> TicketAssignment(string id, TicketViewModel vm)
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
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolution = await _context.TicketResolutions
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Include(t => t.Status)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "FullName");

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
            if (attachment != null && attachment.Length > 0)
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

        // Post: Tickets/AddComment
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

        // POST : Tickets/Resolve
        [HttpPost]
        public async Task<IActionResult> ResolvedConfirmed(string id, TicketViewModel vm) // Add Resolve method
        {
            // Add the current user's ID to the comment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TicketResolution resolution = new(); // Create a new Comment object

            resolution.TicketId = id; // Assign the ticket ID
            resolution.StatusId = vm.StatusId; // Assign the Status ID
            resolution.CreatedOn = DateTime.Now; // Take the current date and time as the resolution creation date
            resolution.CreatedById = userId; // Assign the current user's ID as the creator
            resolution.Description = vm.CommentDescription; // Assign the resolution description entered by the user

            _context.Add(resolution); // Add the new Resolution to the database

            // Update the ticket status
            var ticket = await _context.Tickets
                .Where(x => x.Id == id) // Get the ticket with the specified ID
                .FirstOrDefaultAsync(); // Get the first matching ticket

            ticket.StatusId = vm.StatusId; // Assign the Status ID
            _context.Update(ticket);

            await _context.SaveChangesAsync(); // Save the changes

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Create", // Set the action to "Create" for audit trail
                TimeStamp = DateTime.Now, // Set the current date and time
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), // Get the client's IP address
                UserId = userId, // Assign the current user's ID
                Module = "TicketResolutions", // Assign the module name
                AffectedTable = "TicketResolutions" // Assign the affected table
            };

            _context.Add(activity); // Add the audit trail entry
            await _context.SaveChangesAsync(); // Save the changes

            return RedirectToAction("Resolve", new { id = id }); // Redirect to the Details page after adding the comment description
        }

        // POST : Tickets/Close-Ticket-Confirm
        [HttpPost]
        public async Task<IActionResult> ClosedConfirmed(string id, TicketViewModel vm)
        {
            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
                .FirstOrDefaultAsync();

            // Add the current user's ID to the comment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TicketResolution resolution = new(); // Create a new Comment object

            resolution.TicketId = id; // Assign the ticket ID
            resolution.StatusId = closedstatus.Id; // Assign the Status ID
            resolution.CreatedOn = DateTime.Now; // Take the current date and time as the resolution creation date
            resolution.CreatedById = userId; // Assign the current user's ID as the creator
            resolution.Description = "Ticket Closed"; // Assign the resolution description for closed ticket

            _context.Add(resolution); // Add the new Resolution to the database

            // Update the ticket status
            var ticket = await _context.Tickets
                .Where(x => x.Id == id) // Get the ticket with the specified ID
                .FirstOrDefaultAsync(); // Get the first matching ticket

            ticket.StatusId = closedstatus.Id; // Assign the Status ID
            _context.Update(ticket);

            await _context.SaveChangesAsync(); // Save the changes

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Closed", // Set the action to "Create" for audit trail
                TimeStamp = DateTime.Now, // Set the current date and time
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), // Get the client's IP address
                UserId = userId, // Assign the current user's ID
                Module = "TicketResolutions", // Assign the module name
                AffectedTable = "TicketResolutions" // Assign the affected table
            };

            _context.Add(activity); // Add the audit trail entry
            await _context.SaveChangesAsync(); // Save the changes

            return RedirectToAction("Resolve", new { id = id }); // Redirect to the Details page after adding the comment description
        }

        // POST : Tickets/ReOpen-Ticket-Confirm
        [HttpPost]
        public async Task<IActionResult> ReOpenConfirmed(string id, TicketViewModel vm)
        {
            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "ReOpened")
                .FirstOrDefaultAsync();

            // Add the current user's ID to the comment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TicketResolution resolution = new(); // Create a new Comment object

            resolution.TicketId = id; // Assign the ticket ID
            resolution.StatusId = closedstatus.Id; // Assign the Status ID
            resolution.CreatedOn = DateTime.Now; // Take the current date and time as the resolution creation date
            resolution.CreatedById = userId; // Assign the current user's ID as the creator
            resolution.Description = "Ticket Re-Open"; // Assign the resolution description for closed ticket

            _context.Add(resolution); // Add the new Resolution to the database

            // Update the ticket status
            var ticket = await _context.Tickets
                .Where(x => x.Id == id) // Get the ticket with the specified ID
                .FirstOrDefaultAsync(); // Get the first matching ticket

            ticket.StatusId = closedstatus.Id; // Assign the Status ID
            _context.Update(ticket);

            await _context.SaveChangesAsync(); // Save the changes

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Re-Open", // Set the action to "Create" for audit trail
                TimeStamp = DateTime.Now, // Set the current date and time
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), // Get the client's IP address
                UserId = userId, // Assign the current user's ID
                Module = "TicketResolutions", // Assign the module name
                AffectedTable = "TicketResolutions" // Assign the affected table
            };

            _context.Add(activity); // Add the audit trail entry
            await _context.SaveChangesAsync(); // Save the changes

            return RedirectToAction("Resolve", new { id = id }); // Redirect to the Details page after adding the comment description
        }

        // POST : Tickets/Assign-Ticket
        [HttpPost]
        public async Task<IActionResult> AssignedConfirmed(string id, TicketViewModel vm)
        {
            var reassignedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Assigned")
                .FirstOrDefaultAsync();

            // Add the current user's ID to the comment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TicketResolution resolution = new(); // Create a new Comment object

            resolution.TicketId = id; // Assign the ticket ID
            resolution.StatusId = reassignedstatus.Id; // Assign the Status ID
            resolution.CreatedOn = DateTime.Now; // Take the current date and time as the resolution creation date
            resolution.CreatedById = userId; // Assign the current user's ID as the creator
            resolution.Description = "Ticket Assign"; // Assign the resolution description for assigned ticket

            _context.Add(resolution); // Add the new Resolution to the database

            // Update the ticket status
            var ticket = await _context.Tickets
                .Where(x => x.Id == id) // Get the ticket with the specified ID
                .FirstOrDefaultAsync(); // Get the first matching ticket

            ticket.StatusId = reassignedstatus.Id; // Assign the Status ID
            ticket.AssignedToId = vm.AssignedToId;
            ticket.AssignedOn = DateTime.Now;
            _context.Update(ticket);

            await _context.SaveChangesAsync(); // Save the changes

            //Log the Audit Trail
            var activity = new AuditTrail
            {
                Action = "Closed", // Set the action to "Create" for audit trail
                TimeStamp = DateTime.Now, // Set the current date and time
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), // Get the client's IP address
                UserId = userId, // Assign the current user's ID
                Module = "TicketResolutions", // Assign the module name
                AffectedTable = "TicketResolutions" // Assign the affected table
            };

            _context.Add(activity); // Add the audit trail entry
            await _context.SaveChangesAsync(); // Save the changes

            return RedirectToAction("Resolve", new { id = id }); // Redirect to the Details page after adding the comment description
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