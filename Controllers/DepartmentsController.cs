using System.Security.Claims;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context; // Deklarasi context database dari ApplicationDbContext

        public DepartmentsController(ApplicationDbContext context) // Konstructor
        {
            _context = context; // Konstructor Database
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            // Menampilkan data department
            var applicationDbContext = _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy);

            // Kembali ke halaman index setelah merender data department
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Jika id null maka kembali ke halaman not found
            if (id == null)
            {
                return NotFound();
            }

            // Menampilkan data department
            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            // Jika data department null maka kembali ke halaman not found
            if (department == null)
            {
                return NotFound();
            }

            // Jika data department ada maka kembali ke halaman details
            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            // Menampilkan data department ke halaman create
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] Department department)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Mendapatkan ID Pengguna yang sedang login
            department.CreatedOn = DateTime.Now; // Mendapatkan waktu saat user melakukan aksi create
            department.CreatedById = userId;

            _context.Add(department); // Menambahkan Data
            await _context.SaveChangesAsync(); // Menyimpan Data Ke database

            return RedirectToAction(nameof(Index)); // Pindah ke halaman Index

            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    department.ModifiedOn = DateTime.Now;
                    department.ModifiedById = userId;

                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", department.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", department.ModifiedById);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Mendapatkan data department berdasarkan id
            var department = await _context.Departments.FindAsync(id);

            // Jika data department ada maka akan dihapus
            if (department != null)
            {
                // Menghapus data department
                _context.Departments.Remove(department);
            }

            // Menyimpan perubahan data department
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Fungsi untuk mengecek apakah data department ada atau tidak
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}