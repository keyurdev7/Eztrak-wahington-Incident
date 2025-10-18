using System;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class SupportController : Controller
    {
        public SupportController()
        {
        }


        [HttpGet]
        public ActionResult UserGuide()
        {
            return View();
        }

        [HttpGet]
        public ActionResult QuickGuide()
        {
            return View();
        }
    }
}

