using System;
using System.Text;
using BaiTap_phan3.Function;

namespace BaiTap_phan3.Models
{

    public class NhanVien
    {
        #region Fields
        public static List<NhanVien> NhanViens = new List<NhanVien>();
        #endregion

        #region Properties
        public string MaNhanVien { get; set; }
        public string HoVaTen { get; set; }
        public DateTime NgayThangNamSinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string? ChucVu { get; set; }
        public int SoNamCongTac { get; set; }
        #endregion


        #region Constructors
        public NhanVien()
        {
        }
        public NhanVien(string hoVaTen, DateTime ngayThangNamSinh, string soDienThoai,
                        string diaChi, string chucVu, int soNamCongTac)
        {
            HoVaTen = hoVaTen;
            NgayThangNamSinh = ngayThangNamSinh;
            SoDienThoai = soDienThoai;
            DiaChi = diaChi;
            ChucVu = chucVu;
            SoNamCongTac = soNamCongTac;
        }

        #endregion

        ~NhanVien() { }


        #region Methods

        public void HienThiThongTin()
        {
            Console.WriteLine($"\tMã nhân viên: {MaNhanVien}, Họ và tên: {HoVaTen}, Ngày sinh: {NgayThangNamSinh.ToString("d/M/yyyy")}, " +
                $"Số điện thoại: {SoDienThoai}, Địa chỉ: {DiaChi}, Chức vụ; {ChucVu}, Số năm công tác: {SoNamCongTac}");
        }
        #endregion
    }
}