using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaiTap_phan3.Models
{

    public class NhanVien
    {
        #region Properties
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Họ và tên")]
         [MinLength(3, ErrorMessage ="Chiều dài tối thiểu của tên là 3 kí tự")]
        [MaxLength(50, ErrorMessage ="Chiều dài tối đa của tên là 50 kí tự")]
        [Required(ErrorMessage ="Họ và tên là bắt buộc")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage ="Ngày sinh là bắt buộc")]
        [Display(Name ="Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Display(Name ="Số điện thoại")]
        public string? DienThoai { get; set; }="";
        [Display(Name ="Chức vụ")]
        [MaxLength(20, ErrorMessage ="Chức vụ có chiều dài tối đa là 20 kí tự")]
        public string? ChucVu { get; set; }="";
        [Display(Name ="Phòng ban")]
        [Required(ErrorMessage ="Phòng ban là bắt buộc")]
        public int PhongBanId {get; set;}
        public PhongBan? PhongBan {get; set;} = null;
        #endregion

        #region Constructors
        public NhanVien()
        {
        }
        public NhanVien(string hoVaTen, DateTime ngaySinh, string soDienThoai, string chucVu, PhongBan phongBan)
        {
            HoVaTen = hoVaTen;
            NgaySinh = ngaySinh;
            DienThoai = soDienThoai;
            ChucVu = chucVu;
            PhongBan = phongBan;
        }

        #endregion

        ~NhanVien() { }
    }
}