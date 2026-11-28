using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Models;

namespace TrangSach.Controllers
{
    public abstract class AppController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        protected AppController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected string? Uid => User.FindFirstValue(ClaimTypes.NameIdentifier);

        protected bool IsAdmin => User.IsInRole("Admin");

        protected void Toast(string text) => TempData["toast"] = text;

        protected void ToastNow(string text) => ViewData["toast"] = text;

        protected async Task<ApplicationUser?> MeAsync()
        {
            var id = Uid;
            return id == null ? null : await _userManager.FindByIdAsync(id);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Các thẻ sách cần biết cuốn nào đã được người dùng yêu thích
            var id = Uid;
            if (id != null && HttpContext.Request.Method == "GET")
            {
                ViewData["WishIds"] = _context.Yeuthichs.Where(y => y.Nguoidungid == id)
                    .Select(y => y.Sachid).ToHashSet();
            }
            base.OnActionExecuting(context);
        }
    }
}
