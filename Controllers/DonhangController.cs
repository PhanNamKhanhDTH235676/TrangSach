using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Helpers;
using TrangSach.Models;
using TrangSach.Services;

namespace TrangSach.Controllers
{
    public class DonhangController : AppController
    {
        private readonly DonhangService _dich;

        public DonhangController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DonhangService dich)
            : base(context, userManager)
        {
            _dich = dich;
        }

        public async Task<IActionResult> Index(string? tab)
        {
            var uid = Uid!;
            bool ban = tab == "ban";
            var q = _context.Donhangs.Include(d => d.Sach).ThenInclude(s => s!.Nguoiban)
                .Include(d => d.Nguoimua).AsQueryable();
            q = ban ? q.Where(d => d.Sach!.Nguoibanid == uid) : q.Where(d => d.Nguoimuaid == uid);

            return View(new DonhangIndexVm
            {
                Tab = ban ? "ban" : "mua",
                Items = await q.OrderByDescending(d => d.Ngaydat).ThenByDescending(d => d.Somadon).ToListAsync(),
                Cfg = await _dich.CauhinhAsync()
            });
        }

        private async Task<ThanhtoanVm> TaoVmAsync(int id)
        {
            var uid = Uid!;
            var sach = await _context.Sachs.Include(s => s.Theloai).Include(s => s.Nguoiban)
                .FirstOrDefaultAsync(s => s.Sachid == id);
            var me = await _userManager.FindByIdAsync(uid);
            var cfg = await _dich.CauhinhAsync();

            var vm = new ThanhtoanVm { Sach = sach, Cfg = cfg, SoDuVi = me?.SoDuVi ?? 0 };
            if (sach == null || sach.Trangthai != TrangthaiSach.Dangban || sach.Nguoibanid == uid)
            {
                vm.KhongMua = true;
                return vm;
            }
            vm.PhiMua = DonhangService.Phi(sach.Gia, cfg.Phimua);
            vm.Tong = sach.Gia + vm.PhiMua + cfg.Phivanchuyen;

            var dc = await _context.Diachis.Where(d => d.Nguoidungid == uid)
                .OrderByDescending(d => d.Macdinh).ThenBy(d => d.Diachiid).FirstOrDefaultAsync();
            vm.Diachi = dc != null
                ? $"{dc.Hoten} · {Fmt.PhoneShow(dc.Sodienthoai)} · {dc.Diachichitiet}"
                : $"{me?.HoTen} · {Fmt.PhoneShow(me?.PhoneNumber)}";
            return vm;
        }

        public async Task<IActionResult> Thanhtoan(int id)
        {
            return View(await TaoVmAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Thanhtoan(int id, ThanhtoanForm form)
        {
            var vm = await TaoVmAsync(id);
            if (vm.KhongMua) return View(vm);

            vm.Diachi = form.Diachi ?? "";
            vm.Phuongthuc = Rules.Phuongthuc.Contains(form.Phuongthuc) ? form.Phuongthuc : "vi";
            vm.Dongy = form.Dongy;

            if (!form.Dongy)
            {
                ToastNow("Vui lòng đồng ý điều khoản");
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(form.Diachi))
            {
                ToastNow("Vui lòng nhập địa chỉ nhận sách");
                return View(vm);
            }

            var (don, loi) = await _dich.TaoDonAsync(id, Uid!, form.Diachi.Trim(), vm.Phuongthuc);
            if (don == null)
            {
                ToastNow(loi ?? "Không đặt được đơn hàng");
                var moi = await TaoVmAsync(id);
                if (!moi.KhongMua) Dung(moi, form);
                return View(moi);
            }
            return RedirectToAction(nameof(Thanhcong), new { id = don.Somadon });
        }

        private static ThanhtoanVm Dung(ThanhtoanVm vm, ThanhtoanForm f)
        {
            vm.Diachi = f.Diachi ?? "";
            vm.Phuongthuc = f.Phuongthuc;
            vm.Dongy = f.Dongy;
            return vm;
        }

        public async Task<IActionResult> Thanhcong(int id)
        {
            var don = await _context.Donhangs.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Somadon == id && d.Nguoimuaid == Uid);
            if (don == null) return NotFound();
            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guisach(int id)
        {
            var loi = await _dich.ChuyenAsync(id, 1, Uid!, false);
            Toast(loi ?? "Đã cập nhật đơn hàng");
            return RedirectToAction(nameof(Index), new { tab = "ban" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xacnhan(int id)
        {
            var loi = await _dich.ChuyenAsync(id, 3, Uid!, false);
            Toast(loi ?? "Đã xác nhận nhận sách, tiền được chuyển cho người bán");
            return RedirectToAction(nameof(Index), new { tab = "mua" });
        }
    }
}
