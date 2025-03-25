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
                        label = "City",
                        data,borderWidth = 0,
                        backgroundColor = new[] { "#FF6384", "#36A2EB", "#FFCE56" }
                        }
                }

            };
            return Json(chartData);
        }
    }
}

