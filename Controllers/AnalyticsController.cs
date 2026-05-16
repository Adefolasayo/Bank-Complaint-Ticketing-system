using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketing_system.Models;
using Ticketing_system.Models.Enums;

namespace Ticketing_system.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // =========================
            // TOTAL COMPLAINTS
            // =========================
            var totalComplaints = await _context.Tickets.CountAsync();

            // =========================
            // AVERAGE RESPONSE TIME
            // =========================
            var closedTickets = await _context.Tickets
                .Where(t => t.ClosedAt != null)
                .ToListAsync();

            double avgResponseHours = 0;

            if (closedTickets.Any())
            {
                avgResponseHours = closedTickets
                    .Average(t => (t.ClosedAt.Value - t.CreatedAt).TotalHours);
            }

            // =========================
            // COMPLAINT DISTRIBUTION
            // =========================
            var categoryStats = await _context.Tickets
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // =========================
            // COMPLAINTS OVER TIME
            // =========================
            var monthlyStats = await _context.Tickets
                .GroupBy(t => new
                {
                    t.CreatedAt.Year,
                    t.CreatedAt.Month
                })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // =========================
            // SEND TO VIEW
            // =========================
            ViewBag.TotalComplaints = totalComplaints;

            ViewBag.AvgResponseHours = Math.Round(avgResponseHours, 1); ;

            ViewBag.CategoryLabels =
                categoryStats.Select(x => x.Category).ToList();

            ViewBag.CategoryCounts =
                categoryStats.Select(x => x.Count).ToList();

            ViewBag.MonthLabels =
                monthlyStats
                    .Select(x => $"{x.Month}/{x.Year}")
                    .ToList();

            ViewBag.MonthCounts =
                monthlyStats.Select(x => x.Count).ToList();

            return View();
        }
    }
}
