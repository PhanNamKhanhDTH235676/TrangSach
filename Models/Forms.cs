using System.ComponentModel.DataAnnotations;

namespace TrangSach.Models;

public class SachForm
{
    [Display(Name = "Tên sách")]
    [Required(ErrorMessage = "Vui lòng nhập tên sách")]
    [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự")]
    public string Tensach { get; set; } = "";

    [Display(Name = "Tác giả")]
    [StringLength(120, ErrorMessage = "Tên tác giả tối đa 120 ký tự")]
    public string? Tacgia { get; set; }

    [Display(Name = "ISBN")]
    [StringLength(30, ErrorMessage = "ISBN tối đa 30 ký tự")]
    public string? Isbn { get; set; }

    [Display(Name = "Thể loại")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thể loại")]
    public int Theloaiid { get; set; }

    [Display(Name = "Tình trạng")]
    public string Tinhtrang { get; set; } = "Như mới";

    [Display(Name = "Nhà xuất bản")]
    [StringLength(120, ErrorMessage = "Tên nhà xuất bản tối đa 120 ký tự")]
    public string? Nhaxuatban { get; set; }

    [Display(Name = "Năm xuất bản")]
    [Range(1800, 2100, ErrorMessage = "Năm xuất bản chưa hợp lệ")]
    public int? Namxb { get; set; }

    [Display(Name = "Giá bán")]
    [Required(ErrorMessage = "Vui lòng nhập giá bán")]
    [Range(1000, 100000000, ErrorMessage = "Giá bán từ 1.000đ đến 100.000.000đ")]
    public long? Gia { get; set; }

    [Display(Name = "Mô tả chi tiết")]
    [StringLength(4000, ErrorMessage = "Mô tả tối đa 4000 ký tự")]
    public string? Mota { get; set; }
}

public class ThanhtoanForm
{
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận sách")]
    [StringLength(300)]
    public string Diachi { get; set; } = "";

    public string Phuongthuc { get; set; } = "vi";

    public bool Dongy { get; set; }
}

public class HosoForm
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100)]
    public string HoTen { get; set; } = "";

    [RegularExpression(@"^0(\s?[0-9]){9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số, bắt đầu bằng 0")]
    public string? SoDienThoai { get; set; }

    [StringLength(60)]
    public string? Tinh { get; set; }

    [StringLength(500)]
    public string? Gioithieu { get; set; }
}

public class DoimatkhauForm
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
    public string MatKhauHienTai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
    public string MatKhauMoi { get; set; } = "";
}
