using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Dashboard;

namespace Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get severity counts with names + colors
            var severityData = _context.Incidents
                .GroupBy(i => i.SeverityLevelId)
                .Select(g => new
                {
                    Id = g.Key,
                    Count = g.Count()
                })
                .Join(_context.SeverityLevels, // join with lookup table
                      g => g.Id,
                      s => s.Id,
                      (g, s) => new
                      {
                          Name = s.Name,
                          Color = s.Color,
                          Count = g.Count
                      })
                .ToList();

            // Get status counts with names + colors
            var statusData = _context.Incidents
                .GroupBy(i => i.StatusLegendId)
                .Select(g => new
                {
                    Id = g.Key,
                    Count = g.Count()
                })
                .Join(_context.StatusLegends, // join with lookup table
                      g => g.Id,
                      s => s.Id,
                      (g, s) => new
                      {
                          Name = s.Name,
                          Color = s.Color,
                          Count = g.Count
                      })
                .ToList();

            // Build view model
            var viewModel = new DashboardViewModel
            {
                //SeverityLabels = severityData.Select(s => s.Name).ToList(),
                //SeverityCounts = severityData.Select(s => s.Count).ToList(),
                //SeverityColors = severityData.Select(s => s.Color).ToList(),

                //StatusLabels = statusData.Select(s => s.Name).ToList(),
                //StatusCounts = statusData.Select(s => s.Count).ToList(),
                //StatusColors = statusData.Select(s => s.Color).ToList()
            };

            return View(viewModel);
        }
    }
}
