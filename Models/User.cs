

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BaiTap_phan3.Models
{

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [MaxLength(30, ErrorMessage = "Chiều dài tên không vượt quá 30 kí tự")]
        [MinLength(2, ErrorMessage = "Chiều dài tên không nhỏ hơn 3 kí tự")]
        [Required(ErrorMessage = "Tên là yêu cầu")]
        [Display(Name = "Tên")]
        public string? FirstName { get; set; }
        [MaxLength(30, ErrorMessage = "Chiều dài tên đệm không vượt quá 30 kí tự")]
        [MinLength(2, ErrorMessage = "Chiều dài tên đệm không nhỏ hơn 3 kí tự")]
        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Tên đệm là yêu cầu")]
        public string? LastName { get; set; }
        [MaxLength(30, ErrorMessage = "Chiều dài tên tài khoản không vượt quá 30 kí tự")]
        [MinLength(3, ErrorMessage = "Chiều dài tên tài khoản không nhỏ hơn 3 kí tự")]
        [Required(ErrorMessage = "Tên tài khoản là yêu cầu")]
        [Display(Name = "Tên tài khoản")]
        public string? AccountName { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = false;
        [MaxLength(40, ErrorMessage = "Mật khẩu tối đa 40 kí tự")]
        [MinLength(3, ErrorMessage = "Mật khẩu tối thiểu 3 kí tự")]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là yêu cầu")]
        public string? PassWord { get; set; }
        [NotMapped]
        [Compare("PassWord", ErrorMessage = "Mật khẩu không trùng khớp")]
        [Display(Name = "Mật khẩu xác nhận")]
        public string? ConfirmPassWord { get; set; }
        [NotMapped]
        public virtual IEnumerable<Message>? Messages { get; set; }
        [NotMapped]
        public virtual IEnumerable<MessageRecipient>? MessRecis { get; set; }

        public string FullName()
        {
            return LastName + FirstName;
        }
    }
}