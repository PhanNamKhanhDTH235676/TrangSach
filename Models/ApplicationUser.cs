using Microsoft.AspNetCore.Identity;

namespace TrangSach.Models;

public class ApplicationUser : IdentityUser
{
    public string HoTen { get; set; } = "";
    public string? Tinh { get; set; }
    public string? Gioithieu { get; set; }
    public int NamThamGia { get; set; } = DateTime.Now.Year;
    public bool DaXacMinh { get; set; }

    public long SoDuVi { get; set; }
    public string? TaiKhoanNganHang { get; set; }

    public bool Xacthuc2Buoc { get; set; } = true;
    public bool CanhBaoDangNhap { get; set; } = true;
}
