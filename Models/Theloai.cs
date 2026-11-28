using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangSach.Models;

[Table("theloai")]
public partial class Theloai
{
    [Key]
    public int Theloaiid { get; set; }

    [Display(Name = "Tên thể loại")]
    [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
    [StringLength(60)]
    public string Tentheloai { get; set; } = null!;

    [Display(Name = "Thứ tự")]
    public int Thutu { get; set; }

    public virtual ICollection<Sach> Sachs { get; set; } = new List<Sach>();
}
