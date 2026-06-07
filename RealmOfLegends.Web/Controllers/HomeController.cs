using Microsoft.AspNetCore.Mvc;

namespace RealmOfLegends.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If user is logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            return View();
        }
    }
}
