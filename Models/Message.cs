

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace BaiTap_phan3.Models{

    public class Message{
        [Key]
        public int Id { get; set; }
        public int CreatorId { get; set; }
        [ForeignKey("User")]
        [Display(Name ="Người tạo tin nhắn")]
        public virtual User? Creator {get; set;}
        [MinLength(1, ErrorMessage = "Tin nhắn không được trống")]
        [Display(Name = "Đoạn tin nhắn")]
        public string? MessageBody { get; set; }
        public DateTime CreateDate { get; set; }
        public int ParentMessageId { get; set; }
        [ForeignKey("Message")]
        [Display(Name = "Parent message")]
        public virtual Message? ParentMessage {get; set;}
    }
}