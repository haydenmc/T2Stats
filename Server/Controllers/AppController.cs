using Microsoft.AspNetCore.Mvc;

namespace T2Stats.Controllers
{
    public class AppController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}