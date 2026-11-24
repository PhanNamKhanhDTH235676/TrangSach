using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangSach.Models;

[Table("yeuthich")]
public partial class Yeuthich
{
    [Key]
    public int Yeuthichid { get; set; }

    [StringLength(450)]
    public string Nguoidungid { get; set; } = null!;

    public int Sachid { get; set; }
    public DateTime Ngay { get; set; } = DateTime.Now;

    public virtual ApplicationUser? Nguoidung { get; set; }
    public virtual Sach? Sach { get; set; }
}
