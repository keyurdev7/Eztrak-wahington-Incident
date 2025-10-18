using System;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class TimesheetReportController : Controller
    {
        public TimesheetReportController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

