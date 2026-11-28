using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Helpers;
using TrangSach.Models;

namespace TrangSach.Controllers
{
    public class SachController : AppController
    {
        public SachController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
        }

        private async Task NapFormAsync()
        {
            ViewBag.Theloais = await _context.Theloais.OrderBy(t => t.Thutu).ToListAsync();
            ViewBag.Cauhinh = await _context.Cauhinhs.AsNoTracking().FirstOrDefaultAsync() ?? new Cauhinh();
        }

        public async Task<IActionResult> Create()
        {
            await NapFormAsync();
            return View(new SachForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SachForm form)
        {
            if (!await _context.Theloais.AnyAsync(t => t.Theloaiid == form.Theloaiid))
                ModelState.AddModelError(nameof(form.Theloaiid), "Vui lòng chọn thể loại");
            if (!Rules.Tinhtrang.Contains(form.Tinhtrang))
                ModelState.AddModelError(nameof(form.Tinhtrang), "Tình trạng không hợp lệ");

            if (!ModelState.IsValid)
            {
                await NapFormAsync();
                return View(form);
            }

            var cfg = await _context.Cauhinhs.AsNoTracking().FirstOrDefaultAsync() ?? new Cauhinh();
            var sach = new Sach
            {
                Tensach = form.Tensach.Trim(),
                Tacgia = string.IsNullOrWhiteSpace(form.Tacgia) ? "Chưa rõ" : form.Tacgia.Trim(),
                Theloaiid = form.Theloaiid,
                Gia = form.Gia!.Value,
                Tinhtrang = form.Tinhtrang,
                Nhaxuatban = string.IsNullOrWhiteSpace(form.Nhaxuatban) ? null : form.Nhaxuatban.Trim(),
                Namxb = form.Namxb,
                Isbn = string.IsNullOrWhiteSpace(form.Isbn) ? null : form.Isbn.Trim(),
                Mota = string.IsNullOrWhiteSpace(form.Mota) ? "Tin đăng của thành viên." : form.Mota.Trim(),
                Trangthai = cfg.Duyettruoc ? TrangthaiSach.Choduyet : TrangthaiSach.Dangban,
                Ngaydang = DateTime.Now,
                Nguoibanid = Uid!
            };
            _context.Sachs.Add(sach);
            await _context.SaveChangesAsync();

            Toast(cfg.Duyettruoc ? "Đã đăng, đang chờ kiểm duyệt" : "Đã đăng bán sách");
            return RedirectToAction("Details", "Main", new { id = sach.Sachid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Doitrangthai(int id)
        {
            var sach = await _context.Sachs.FirstOrDefaultAsync(s => s.Sachid == id && s.Nguoibanid == Uid);
            if (sach == null) return NotFound();

            if (sach.Trangthai == TrangthaiSach.Dangban)
            {
                sach.Trangthai = TrangthaiSach.Daan;
                Toast("Đã ẩn tin đăng");
            }
            else if (sach.Trangthai == TrangthaiSach.Daan)
            {
                sach.Trangthai = TrangthaiSach.Dangban;
                Toast("Đã hiện lại tin đăng");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Hoso", new { tab = "tin" });
        }
    }
}
