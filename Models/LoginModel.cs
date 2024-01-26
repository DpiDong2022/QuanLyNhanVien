

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BaiTap_phan3.Models{

    public class LoginModel{
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage ="Tên tài khoản là yêu cầu")]
        public string AccountName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage ="Mật khẩu là yêu cầu")]
        public string Password { get; set; }
    }
}