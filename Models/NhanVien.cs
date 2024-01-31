using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BaiTap_phan3.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaiTap_phan3.Models
{



    [MyActionFilter]
    public class NhanVien
    {
        #region Properties
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Họ và tên")]
        [MinLength(3, ErrorMessage ="Chiều dài tối thiểu của tên là 3 kí tự")]
        [MaxLength(256, ErrorMessage ="Chiều dài tối đa của tên là 256 kí tự")]
        [Required(ErrorMessage ="Họ và tên là bắt buộc")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage ="Ngày sinh là bắt buộc")]
        [Display(Name ="Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Display(Name ="Số điện thoại")]
        public string? DienThoai { get; set; }="";

        [Display(Name ="Chức vụ")]
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage = "Chức vụ là yêu cầu")]
        public int ChucVuId {get; set;}
        public ChucVu? ChucVu {get; set;}

        [Display(Name ="Phòng ban")]
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage = "Phòng ban là yêu cầu")]
        public int PhongBanId {get; set;}
        public PhongBan? PhongBan {get; set;} = null;
        #endregion

        #region Constructors
        public NhanVien()
        {
        }
        public NhanVien(string hoVaTen, DateTime ngaySinh, int chucVuId, string soDienThoai, ChucVu chucVu, PhongBan phongBan)
        {
            HoVaTen = hoVaTen;
            NgaySinh = ngaySinh;
            DienThoai = soDienThoai;
            ChucVu = chucVu;
            ChucVuId = chucVuId;
            PhongBan = phongBan;
        }

        #endregion

        ~NhanVien() { }
    }
}