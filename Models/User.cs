

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BaiTap_phan3.Models{

    public class User{
        [Key]
        public int Id { get; set; }
        [MaxLength(30, ErrorMessage = "Chiều dài tên không vượt quá 30 kí tự")]
        [MinLength(3, ErrorMessage = "Chiều dài tên không nhỏ hơn 3 kí tự")]
        [Required(ErrorMessage = "Tên là yêu cầu")]
        [Display(Name = "Tên")]
        public string? FirstName { get; set; }
        [MaxLength(30, ErrorMessage = "Chiều dài tên đệm không vượt quá 30 kí tự")]
        [MinLength(3, ErrorMessage = "Chiều dài tên đệm không nhỏ hơn 3 kí tự")]
        [Display(Name="Họ")]
        [Required(ErrorMessage = "Tên đệm là yêu cầu")]
        public string? LastName { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = false;
        public IEnumerable<Message>? Messages {get; set;}
        public IEnumerable<MessageRecipient>? MessageRecipients {get; set;}
    }
}