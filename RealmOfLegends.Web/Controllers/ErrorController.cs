using Microsoft.AspNetCore.Mvc;

namespace RealmOfLegends.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            ViewData["Title"] = "Page Not Found";
            return View();
        }

        [Route("Error/403")]
        public IActionResult Error403()
        {
            Response.StatusCode = 403;
            ViewData["Title"] = "Access Denied";
            return View();
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            ViewData["Title"] = "Server Error";
            return View();
        }

        [Route("Error/{code:int}")]
        public IActionResult ErrorCode(int code)
        {
            ViewData["Title"] = $"Error {code}";
            ViewBag.ErrorCode = code;
            return View("Error");
        }
    }
}
