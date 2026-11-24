using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.WebUtilities;

namespace TrangSach.Models;

public record NguoiBanStat(double? Diem, int SoDanhGia, int SoGiaoDich);

public class TheloaiDem
{
    public string Ten { get; set; } = "";
    public int Soluong { get; set; }
}

public class TrangchuVm
{
    public List<TheloaiDem> Theloais { get; set; } = new();
    public List<Sach> SachMoi { get; set; } = new();
    public List<Donhang> GiaoDichGanDay { get; set; } = new();
    public int SoSach { get; set; }
    public int SoThanhVien { get; set; }
    public double? DiemTb { get; set; }
    public int Phiban { get; set; }
    public int Phimua { get; set; }
}

public class DanhsachVm
{
    public const int CoTrang = 8;

    public List<Sach> Sachs { get; set; } = new();
    public int Tong { get; set; }
    public int Trang { get; set; } = 1;
    public int TongTrang { get; set; } = 1;
    public string? Q { get; set; }
    public string? Theloai { get; set; }
    public List<string> Tinhtrang { get; set; } = new();
    public long? Giatu { get; set; }
    public long? Giaden { get; set; }
    public string Sapxep { get; set; } = "new";
    public string Kieu { get; set; } = "grid";
    public List<Theloai> Theloais { get; set; } = new();

    public string Url(int trang)
    {
        var p = new List<KeyValuePair<string, string?>>();
        if (!string.IsNullOrEmpty(Q)) p.Add(new("q", Q));
        if (!string.IsNullOrEmpty(Theloai)) p.Add(new("theloai", Theloai));
        foreach (var t in Tinhtrang) p.Add(new("tinhtrang", t));
        if (Giatu != null) p.Add(new("giatu", Giatu.ToString()));
        if (Giaden != null) p.Add(new("giaden", Giaden.ToString()));
        if (Sapxep != "new") p.Add(new("sapxep", Sapxep));
        if (Kieu != "grid") p.Add(new("kieu", Kieu));
        if (trang > 1) p.Add(new("trang", trang.ToString()));
        return QueryHelpers.AddQueryString("/Main/Danhsach", p);
    }
}

public class ChitietVm
{
    public Sach Sach { get; set; } = null!;
    public bool LaChuSach { get; set; }
    public bool DaYeuThich { get; set; }
    public NguoiBanStat Nb { get; set; } = new(null, 0, 0);
    public List<Danhgia> Danhgias { get; set; } = new();
    public List<Sach> Tuongtu { get; set; } = new();
}

public class ThanhtoanVm
{
    public Sach? Sach { get; set; }
    public bool KhongMua { get; set; }
    public Cauhinh Cfg { get; set; } = new();
    public long PhiMua { get; set; }
    public long Tong { get; set; }
    public long SoDuVi { get; set; }
    public string Diachi { get; set; } = "";
    public string Phuongthuc { get; set; } = "vi";
    public bool Dongy { get; set; }
}

public class DonhangIndexVm
{
    public string Tab { get; set; } = "mua";
    public List<Donhang> Items { get; set; } = new();
    public Cauhinh Cfg { get; set; } = new();
}

public class ThangDoanhThu
{
    public string Nhan { get; set; } = "";
    public long GiaTri { get; set; }
}

public class HosoVm
{
    public ApplicationUser Nguoi { get; set; } = null!;
    public bool LaToi { get; set; }
    public string Tab { get; set; } = "tongquan";
    public NguoiBanStat Stat { get; set; } = new(null, 0, 0);
    public long DoanhThu30 { get; set; }
    public long PhiDaTra { get; set; }
    public int DonCanGiao { get; set; }
    public List<ThangDoanhThu> BieuDo { get; set; } = new();
    public List<Donhang> DonGanDay { get; set; } = new();
    public List<Sach> TinDang { get; set; } = new();
    public List<Danhgia> Danhgias { get; set; } = new();
}

public class CaidatVm
{
    public string Muc { get; set; } = "hoso";
    public ApplicationUser Me { get; set; } = null!;
    public List<Diachi> Diachis { get; set; } = new();
    public string? Dienthoai { get; set; }
}

// ===== Quản trị =====

public record AdminChip(string Key, string Nhan, int Soluong);

public record DrawerAction(string Text, string Css, string Action, string RouteId, bool Danger = false);

public class DrawerVm
{
    public string Nhan { get; set; } = "";
    public string Tieude { get; set; } = "";
    public IHtmlContent? Dau { get; set; }
    public List<KeyValuePair<string, IHtmlContent>> Dong { get; set; } = new();
    public List<DrawerAction> Nut { get; set; } = new();
    public string? LienKet { get; set; }
    public string DongUrl { get; set; } = "";
    public string? F { get; set; }
    public string? Q { get; set; }
}

public interface IAdminList
{
    string Action { get; }
    string Tieude { get; }
    string? F { get; }
    string? Q { get; }
    List<AdminChip> Chips { get; }
    int TongTatCa { get; }
    int Dem { get; }
}

public class AdminListVm<T> : IAdminList
{
    public int Dem => Items.Count;
    public string Action { get; set; } = "";
    public string Tieude { get; set; } = "";
    public string? F { get; set; }
    public string? Q { get; set; }
    public List<AdminChip> Chips { get; set; } = new();
    public int TongTatCa { get; set; }
    public List<T> Items { get; set; } = new();
    public DrawerVm? Drawer { get; set; }
    public Dictionary<string, NguoiBanStat> Stats { get; set; } = new();
}

public class AdminKpi
{
    public string Nhan { get; set; } = "";
    public string GiaTri { get; set; } = "";
    public string? Nhandien { get; set; }
    public double[] Spark { get; set; } = Array.Empty<double>();
}

public class AdminQuick
{
    public string Nhan { get; set; } = "";
    public int Soluong { get; set; }
    public string Action { get; set; } = "";
    public string F { get; set; } = "";
}

public class AdminHomeVm
{
    public List<AdminKpi> Kpis { get; set; } = new();
    public double[] GmvTuan { get; set; } = Array.Empty<double>();
    public List<TheloaiDem> TheoTheloai { get; set; } = new();
    public List<AdminQuick> Quick { get; set; } = new();
    public List<Donhang> DonGanDay { get; set; } = new();
}

public class AdminCaidatVm
{
    public Cauhinh Cfg { get; set; } = new();
    public List<Theloai> Theloais { get; set; } = new();
}
