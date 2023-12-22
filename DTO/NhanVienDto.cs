
using BaiTap_phan3.Models;

namespace BaiTap_phan3.DTO{

    public class NhanVienDto{
        public string HoVaTen { get; set; } = "";
        public DateTime NgaySinh { get; set; }
        public string? DienThoai { get; set; }
        public string? ChucVu { get; set; }
        public int? PhongBanId { get; set; }
        public PhongBan? PhongBan {get; set;}
    }
}