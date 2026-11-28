using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrangSach.Models;

namespace TrangSach.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Status(int code)
        {
            ViewBag.Code = code;
            Response.StatusCode = code;
            return View();
        }
    }
}
