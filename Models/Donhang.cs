using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangSach.Models;

[Table("donhang")]
public partial class Donhang
{
    [Key]
    public int Donhangid { get; set; }

    public int Somadon { get; set; }

    public int Sachid { get; set; }

    [StringLength(450)]
    public string Nguoimuaid { get; set; } = null!;

    public long Giasach { get; set; }
    public long Phinguoiban { get; set; }
    public long Phinguoimua { get; set; }
    public long Phivanchuyen { get; set; }
    public long Tongtien { get; set; }

    public int Trangthai { get; set; }
    public DateTime Ngaydat { get; set; } = DateTime.Now;

    [StringLength(300)]
    public string? Diachigiao { get; set; }

    [StringLength(10)]
    public string Phuongthuc { get; set; } = "vi";

    [NotMapped]
    public string Ma => "TS-" + Somadon;

    public virtual Sach? Sach { get; set; }
    public virtual ApplicationUser? Nguoimua { get; set; }
}
