using HelpDeskSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HelpDeskSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toasty;

        public DataController(ApplicationDbContext context, IToastNotification toasty)
        {
            _context = context;
            _toasty = toasty;
        }


        // GET: api/<DataController>
        [HttpGet]
        //[AllowAnonymous]
        public async Task<JsonResult> GetTicketSubCategories(int Id)
        {
            try
            {
                var subcategories = await _context
                    .TicketSubCategory
                    .Where(x => x.CategoryId == Id)
                    .OrderBy(c => c.Name)
                    .Select(I => new { Id = I.Id, Name = I.Name })
                    .Distinct()
                    .ToListAsync();

                return Json(subcategories);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET api/<DataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
