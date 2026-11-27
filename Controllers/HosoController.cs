using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Models;
using TrangSach.Services;

namespace TrangSach.Controllers
{
    public class HosoController : AppController
    {
        private readonly ThongkeService _thongke;

        public HosoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ThongkeService thongke)
            : base(context, userManager)
        {
            _thongke = thongke;
        }

        public async Task<IActionResult> Index(string? id, string? tab)
        {
            var uid = Uid!;
            bool laToi = string.IsNullOrEmpty(id) || id == uid;
            var nguoi = await _userManager.FindByIdAsync(laToi ? uid : id!);
            if (nguoi == null) return NotFound();

            var vm = new HosoVm
            {
                Nguoi = nguoi,
                LaToi = laToi,
                Stat = await _thongke.NguoiBanAsync(nguoi.Id),
                Danhgias = await _context.Danhgias.Include(d => d.Nguoidanhgia)
                    .Where(d => d.Nguoibanid == nguoi.Id).OrderByDescending(d => d.Ngay).ToListAsync()
            };

            // Người khác chỉ thấy tin đang bán; chủ tài khoản thấy mọi tin của mình
            var tin = _context.Sachs.Include(s => s.Theloai).Where(s => s.Nguoibanid == nguoi.Id);
            if (!laToi) tin = tin.Where(s => s.Trangthai == TrangthaiSach.Dangban);
            vm.TinDang = await tin.OrderByDescending(s => s.Ngaydang).ToListAsync();

            if (!laToi)
            {
                vm.Tab = tab == "danhgia" ? "danhgia" : "tin";
                return View(vm);
            }

            vm.Tab = tab == "tin" || tab == "danhgia" ? tab : "tongquan";

            var now = DateTime.Now;
            var t30 = now.AddDays(-30);

            // Doanh thu 30 ngày: tiền người bán thực nhận từ các đơn đã hoàn tất
            vm.DoanhThu30 = await _context.Donhangs
                .Where(d => d.Sach!.Nguoibanid == uid && d.Trangthai == 3 && d.Ngaydat >= t30)
                .SumAsync(d => (long?)(d.Giasach - d.Phinguoiban)) ?? 0;

            // Phí môi giới đã trả: phí người bán (đơn hoàn tất) + phí bảo vệ người mua (đơn không hoàn tiền)
            long phiBan = await _context.Donhangs
                .Where(d => d.Sach!.Nguoibanid == uid && d.Trangthai == 3)
                .SumAsync(d => (long?)d.Phinguoiban) ?? 0;
            long phiMua = await _context.Donhangs
                .Where(d => d.Nguoimuaid == uid && d.Trangthai != 4)
                .SumAsync(d => (long?)d.Phinguoimua) ?? 0;
            vm.PhiDaTra = phiBan + phiMua;

            vm.DonCanGiao = await _context.Donhangs.CountAsync(d => d.Sach!.Nguoibanid == uid && d.Trangthai == 0);

            // Biểu đồ 6 tháng gần nhất (nghìn đồng)
            var dau = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var dt = await _context.Donhangs
                .Where(d => d.Sach!.Nguoibanid == uid && d.Trangthai == 3 && d.Ngaydat >= dau)
                .Select(d => new { d.Ngaydat, Nhan = d.Giasach - d.Phinguoiban })
                .ToListAsync();
            for (int i = 0; i < 6; i++)
            {
                var th = dau.AddMonths(i);
                vm.BieuDo.Add(new ThangDoanhThu
                {
                    Nhan = "T" + th.Month,
                    GiaTri = dt.Where(x => x.Ngaydat.Year == th.Year && x.Ngaydat.Month == th.Month).Sum(x => x.Nhan)
                });
            }

            vm.DonGanDay = await _context.Donhangs.Include(d => d.Sach)
                .Where(d => d.Nguoimuaid == uid).OrderByDescending(d => d.Ngaydat).Take(3).ToListAsync();
            return View(vm);
        }
    }
}
