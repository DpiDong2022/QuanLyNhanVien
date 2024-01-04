
using BaiTap_phan3.Models;

namespace BaiTap_phan3.DTO{

    public class NhanVienDto{
        public NhanVienDto()
        {
        }

        public int Id {get; set;}
        public string HoVaTen { get; set; } = "";
        public DateTime NgaySinh { get; set; }
        public string? DienThoai { get; set; }
        public string? ChucVu { get; set; }
        public PhongBan PhongBan { get; set; }

        
    }

}