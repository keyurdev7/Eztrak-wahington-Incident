using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class IncidentProgressController : Controller
    {

        [HttpGet]
        public async Task<ActionResult> Index(long id)
        {
            return View();
        }
    }
}
