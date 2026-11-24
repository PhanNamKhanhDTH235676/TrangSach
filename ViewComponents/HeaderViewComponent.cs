using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Models;

namespace TrangSach.ViewComponents;

public class HeaderVm
{
    public bool DangNhap { get; set; }
    public ApplicationUser? Me { get; set; }
    public bool IsAdmin { get; set; }
    public int WishCount { get; set; }
    public List<Theloai> Theloais { get; set; } = new();
    public string Controller { get; set; } = "";
    public string Action { get; set; } = "";
    public string? Theloai { get; set; }
    public string? Q { get; set; }
}

public class HeaderViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public HeaderViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var vm = new HeaderVm
        {
            Controller = (string?)ViewContext.RouteData.Values["controller"] ?? "",
            Action = (string?)ViewContext.RouteData.Values["action"] ?? "",
            DangNhap = User.Identity?.IsAuthenticated == true
        };

        if (vm.DangNhap)
        {
            vm.Me = await _users.GetUserAsync(HttpContext.User);
            vm.IsAdmin = HttpContext.User.IsInRole("Admin");
            if (vm.Me != null)
            {
                vm.WishCount = await _db.Yeuthichs.CountAsync(y => y.Nguoidungid == vm.Me.Id);
                vm.Theloais = await _db.Theloais.OrderBy(t => t.Thutu).ToListAsync();
            }
            else
            {
                vm.DangNhap = false;
            }
        }

        // Ô tìm kiếm chỉ mang theo bộ lọc khi đang ở trang danh sách sách
        if (vm.Controller == "Main" && vm.Action == "Danhsach")
        {
            vm.Theloai = Request.Query["theloai"].ToString();
            vm.Q = Request.Query["q"].ToString();
        }
        return View(vm);
    }
}
