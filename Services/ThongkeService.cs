using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Models;

namespace TrangSach.Services;

public class ThongkeService
{
    private readonly ApplicationDbContext _db;

    public ThongkeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<NguoiBanStat> NguoiBanAsync(string userId)
    {
        int n = await _db.Danhgias.CountAsync(d => d.Nguoibanid == userId);
        double? tb = n == 0 ? null : await _db.Danhgias.Where(d => d.Nguoibanid == userId).AverageAsync(d => (double)d.Sao);
        int gd = await _db.Donhangs.CountAsync(d => d.Trangthai == 3 && d.Sach!.Nguoibanid == userId);
        return new NguoiBanStat(tb, n, gd);
    }

    public async Task<Dictionary<string, NguoiBanStat>> TatCaAsync()
    {
        var dg = await _db.Danhgias
            .GroupBy(d => d.Nguoibanid)
            .Select(g => new { Id = g.Key, N = g.Count(), Tb = g.Average(x => (double)x.Sao) })
            .ToListAsync();
        var gd = await _db.Donhangs.Where(d => d.Trangthai == 3)
            .GroupBy(d => d.Sach!.Nguoibanid)
            .Select(g => new { Id = g.Key, N = g.Count() })
            .ToListAsync();

        var kq = new Dictionary<string, NguoiBanStat>();
        foreach (var a in dg) kq[a.Id] = new NguoiBanStat(a.Tb, a.N, 0);
        foreach (var b in gd)
        {
            kq.TryGetValue(b.Id, out var cu);
            kq[b.Id] = new NguoiBanStat(cu?.Diem, cu?.SoDanhGia ?? 0, b.N);
        }
        return kq;
    }

    public static readonly NguoiBanStat Trong = new(null, 0, 0);
}
