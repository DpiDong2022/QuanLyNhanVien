using System;
using System.Text;

namespace BaiTap_phan3.Models
{

    public class NhanVien
    {
        #region Properties
        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? DienThoai { get; set; }
        public string? ChucVu { get; set; }
        public int? PhongBanId { get; set; }=0;
        public PhongBan? PhongBan {get; set;}
        #endregion


        #region Constructors
        public NhanVien()
        {
        }
        public NhanVien(string hoVaTen, DateTime ngaySinh, string soDienThoai, string chucVu)
        {
            HoVaTen = hoVaTen;
            NgaySinh = ngaySinh;
            DienThoai = DienThoai;
            ChucVu = chucVu;
        }

        #endregion

        ~NhanVien() { }
    }
}