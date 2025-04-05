using System.Data;
using HelpDeskSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskSystem.Controllers
{
    public class ChartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetChartData()
        {
            var genderCounts = _context.Users
                .GroupBy(u => string.IsNullOrEmpty(u.Gender) ? "Unknown" : u.Gender)
                .Select(g => new
                {
                    Gender = g.Key,
                    Count = g.Count() // Hitung jumlah user per gender
                })
                .ToList();

            var labels = genderCounts.Select(g => g.Gender).ToArray(); // ["Male", "Female"]
            var data = genderCounts.Select(g => g.Count).ToArray(); // [50, 30]

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                new
                {


                    label = "Users",
                    data,
                    borderWidth = 0
                }
            }
            };

            return Json(chartData);
        }

        [HttpGet]
        public IActionResult GetCountryDataChart()
        {
            var cityCounts = _context.Users
                .GroupBy(u => string.IsNullOrEmpty(u.Country) ? "Unknown" : u.Country)
                .Select(g => new
                {
                    Country = g.Key,
                    Count = g.Count() // Hitung jumlah user per country
                })
                .ToList();

            var labels = cityCounts.Select(g => g.Country).ToArray();
            var data = cityCounts.Select(g => g.Count).ToArray();

            var chartData = new
            {
                labels,
                datasets = new[]
                {
                    new
                        {
                        label = "city",
                        data,
                        borderWidth = 0,
                        backgroundColor = new[] { "#FF6384", "#36A2EB", "#FFCE56" }
                        }
                }

            };
            return Json(chartData);
        }

        [HttpGet]
        public IActionResult GetTicketDataChart()
        {
            var ticketCounts = _context.Tickets
                .GroupBy(u => string.IsNullOrEmpty(u.Title) ? "Unknown" : u.Title)
                .Select(g => new
                {
                    Title = g.Key,
                    Count = g.Count() // Hitung jumlah user per country
                })
                .ToList();

            var labels = ticketCounts.Select(g => g.Title).ToArray();
            var data = ticketCounts.Select(g => g.Count).ToArray();

            var chartData = new
            {
                labels,
                datasets = new[]
                {
            new
            {
                label = "Tickets",
                data,
                borderWidth = 2,  // Agar garis terlihat
                borderColor = "#AC5947",  // Warna garis
                backgroundColor = "rgba(172, 89, 71, 0.2)", // Warna fill bawah garis
                fill = true
            }
                }
            };
            return Json(chartData);
        }
    }
}

